using System;

namespace food_delivery;

public class Restaurant : IRestaurant
{
    private readonly IKitchen kitchen;
    private ICourier courier = null!;
    private Order order = null!;

    public IKitchen Kitchen => kitchen;

    public ICourier Courier => courier;

    public Restaurant(IKitchen kitchen)
    {
        this.kitchen = kitchen;
    }

    public void TakeOrder(Order order)
    {
        this.order = order;
        this.order.UpdateStatus(OrderStatus.InProgress);
    }

    public void ServeOrder()
    {
        throw new NotImplementedException();
    }

    public void StartDelivering()
    {
        throw new NotImplementedException();
    }

    public void DeliverOrder()
    {
        throw new NotImplementedException();
    }

    public void AssignCourier(ICourier courier)
    {
        throw new NotImplementedException();
    }

    public bool HasCourier() => courier != null;
    public bool HasOrder() => order != null;
}
