using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using Core.Entities.OrderAggregate;

namespace API.Extensions
{
    public static class OrderMappingExtensions
    {
        public static OrderDto ToDto(this Order order){
            return new OrderDto{
                Id = order.Id,
                BuyerEmail = order.BuyerEmail,
                DeliveryMethod = order.DeliveryMethod.Description,
                OrderDate = order.OrderDate,
                ShippingAddress = order.ShippingAddress,
                PaymentSummary = order.PaymentSummary,
                ShippingPrice = order.DeliveryMethod.Price,
                OrderItems = order.OrderItems.Select(i=> i.ToDto()).ToList(),
                SubTotal = order.SubTotal,
                Total = order.GetTotal(),
                Status = order.Status.ToString(),
                PaymentIntentId = order.PaymentIntentId
            }
        }
    }

    public static OrderItemDto ToDto(this OrderItem orderItem){
        return new OrderItemDto{
            ProductId = orderItem.ItemOrdered.ProductId,
            PictureUrl = orderItem.ItemOrdered.PictureUrl,
            ProductName = orderItem.ItemOrdered.ProductName,
            Price = orderItem.Price,
            Quantity = orderItem.Quantity
        }
    }
}