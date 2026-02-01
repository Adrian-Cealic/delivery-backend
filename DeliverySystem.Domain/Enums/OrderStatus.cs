namespace DeliverySystem.Domain.Enums;

public enum OrderStatus
{
    Created = 0,
    Confirmed = 1,
    Processing = 2,
    ReadyForDelivery = 3,
    InDelivery = 4,
    Delivered = 5,
    Cancelled = 6
}
