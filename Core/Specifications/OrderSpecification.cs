using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string email) : base(x=> x.BuyerEmail == email)
        {
            AddInclude(x=> x.DeliveryMethod);
            AddInclude(x=> x.OrderItems);
            AddOrderByDescending(x=> x.OrderDate);
        }

        public OrderSpecification(string email , int id) : base(x=> x.BuyerEmail == email && x.Id == id)
        {
            AddInclude(x=> x.DeliveryMethod);
            AddInclude(x=> x.OrderItems);
        }

        public OrderSpecification(string paymentIntentId , bool isPaymentIntent) : base(x=> x.PaymentIntentId == paymentIntentId)
        {
            AddInclude(x=> x.DeliveryMethod);
            AddInclude(x=> x.OrderItems);
        }
    }
}