using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController(IPaymentService paymentRepo , IUnitOfWork unit ,
        ILogger<PaymentsController> logger , IConfiguration config , IHubContext<NotificationHub> hubContext) : BaseController
    {
        private readonly string _whSecret = config["StripeSettings:WhSecret"];

        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await paymentRepo.CreateOrUpdatePaymentIntent(cartId);
            if(cart is null) return BadRequest("Problem with your cart");

            return Ok(cart);
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await unit.Repository<DeliveryMethod>().GetAllAsync());
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook(){
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent =ConstructStripeEvent(json); 
                if(stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    return BadRequest("Invalid event data");
                }

                await HandlePaymentIntentSucceeded(intent);

                return Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "webhook error");
                return StatusCode(StatusCodes.Status500InternalServerError,"Stripe webhook error");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occureed");
                return StatusCode(StatusCodes.Status500InternalServerError,"An unexpected error occureed");
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            if(intent.Status == "succeeded"){

                var spec = new OrderSpecification(intent.Id , true);
                var order = await unit.Repository<Core.Entities.OrderAggregate.Order>().GetEntityWithSpec(spec)
                            ?? throw new Exception("Order not found");

                if((long)order.GetTotal() * 100 != intent.Amount)
                {
                    order.Status = OrderStatus.PaymentMismatch;
                }
                else
                {
                    order.Status = OrderStatus.PaymentRecevied;
                }

                await unit.Complete();

                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
                if(!string.IsNullOrEmpty(connectionId))
                {
                    await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification",order.ToDto());
                }
            }
        }

        private Event ConstructStripeEvent(string json)
        {
            try
            {
                return EventUtility.ConstructEvent(json,Request.Headers["Stripe-Signature"],_whSecret);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Failed to construct stripe event");
                throw new StripeException("Invalid Signature");
            }
        }
    }
}