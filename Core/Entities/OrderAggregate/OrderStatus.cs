namespace Core.Entities.OrderAggregate
{
    public enum OrderStatus
    {
        Pending,
        PaymentRecevied,
        PaymentFaild,
        PaymentMismatch
    }
}