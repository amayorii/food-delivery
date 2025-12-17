using System;

namespace food_delivery;

public interface IRestaurant
{
    public ICourier Courier { get; }
    public void TakeOrder(Order order);
    public void ServeOrder();
    public void StartDelivering();
    public void DeliverOrder();
    public void AssignCourier(ICourier courier);
    public bool HasCourier();
    public bool HasOrder();
}
