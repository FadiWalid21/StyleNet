using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Authorize]
    public class OrderController(ICartRepository cartService, IUnitOfWork unit) : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            var email = User.GetEmail();

            var cart = await cartService.GetCartAsync(orderDto.CartId);
            if (cart is null) return BadRequest("Cart not found");

            if (cart.PaymentIntentId is null) return BadRequest("No payment intent for this order");

            var items = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
                if (productItem is null) return BadRequest("Problem with the order");

                var itemOrderd = new ProductItemOrdered
                {
                    ProductId = item.ProductId,
                    PictureUrl = item.PictureUrl,
                    ProductName = item.ProductName
                };

                var orderItem = new OrderItem
                {
                    ItemOrdered = itemOrderd,
                    Price = item.Price,
                    Quantity = item.Quantity
                };

                items.Add(orderItem);
            }

            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod is null) return BadRequest("No delivery method selected");

            var order = new Order
            {
                DeliveryMethod = deliveryMethod,
                OrderItems = items,
                ShippingAddress = orderDto.ShippingAddress,
                SubTotal = items.Sum(i => i.Price * i.Quantity),
                PaymentSummary = orderDto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId,
                BuyerEmail = email
            };

            unit.Repository<Order>().Add(order);

            if (await unit.Complete()) return Ok(order);

            return BadRequest("Problem creating the order.");

        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrderForUser()
        {
            var spec = new OrderSpecification(User.GetEmail());

            var orders = await unit.Repository<Order>().ListAsync(spec);

            var ordersToReturn = orders.Select(o=> o.ToDto()).ToList();
            return Ok(ordersToReturn);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(User.GetEmail(),id);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);

            if(order is null) return NotFound();

            return Ok(order.ToDto());
        }
    }
}