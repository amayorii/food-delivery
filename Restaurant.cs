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

        if(HasOrder() && HasCourier())
        {
            if(!Kitchen.HasFreeCooks())
            {
                throw new InvalidOperationException("Cannot serve order. No free cooks available.");
            }
            foreach(var item in order.Items)
            {
                // get dish name: we have IFood.ClassName when getting type, so we need to split by . and take the last part
                string name = item.GetType().ToString().Split('.').Last();

                if(Kitchen.HasIngredients(name))
                {
                    var dish = Kitchen.CookDish(name);
                    Courier.Bag.Add(dish);
                }
                else
                {
                    throw new InvalidOperationException($"Cannot serve order. Missing ingredients for {name}.");
                }
            }

            order.UpdateStatus(OrderStatus.Ready);
        }
    }

    public void StartDelivering()
    {
        if (HasOrder() && HasCourier())
        {
            courier.Status = "Delivering";
            order.UpdateStatus(OrderStatus.Delivering);
        }
    }

    public void DeliverOrder()
    {
        if (order.Status == OrderStatus.Delivering)
        {
            order.UpdateStatus(OrderStatus.Delivered);
            courier.Status = "Available";
            courier = null!;
            order = null!;
        }
    }

    public void AssignCourier(ICourier courier)
    {
        if (courier.Status == "Available" && this.courier == null)
        {
            this.courier = courier;
            this.courier.Status = "On order";
            order.CourierId = Courier.Id;
            order.UpdateStatus(OrderStatus.InProgress);
        }
    }

    public bool HasCourier() => courier != null;
    public bool HasOrder() => order != null;

   
}
