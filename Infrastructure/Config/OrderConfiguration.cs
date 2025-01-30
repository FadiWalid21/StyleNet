using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o=> o.ShippingAddress, o=> o.WithOwner());
            builder.OwnsOne(o=> o.PaymentSummary, o=> o.WithOwner());
            builder.Property(p=> p.Status).HasConversion(
                o=> o.ToString(),
                o=> (OrderStatus)Enum.Parse(typeof(OrderStatus),o)
            );
            builder.Property(o=> o.SubTotal).HasColumnType("decimal(18,2)");
            builder.HasMany(o=> o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Property(p=> p.OrderDate).HasConversion(
                o=> o.ToUniversalTime(),
                o=> DateTime.SpecifyKind(o,DateTimeKind.Utc)
            );
        }
    }
}