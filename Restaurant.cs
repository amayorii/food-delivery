using System;
using NLog;

namespace food_delivery;

public class Restaurant : IRestaurant
{
    // LOGGER
    private readonly static Logger logger = LogManager.GetLogger("myAppLoggerRule");

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
        logger.Info($"Order with id {order.Id} received and set to InProgress.");
    }

    public void ServeOrder()
    {
        logger.Info("Trying to serve order.");
        if (HasOrder() && HasCourier())
        {
            logger.Info($"Serving order with id {order.Id}.");
            if (!Kitchen.HasFreeCooks())
            {
                logger.Error($"Cannot serve order with id {order.Id}. No free cooks available.");
                throw new InvalidOperationException("Cannot serve order. No free cooks available.");
            }
            foreach (var item in order.Items)
            {
                // get dish name: we have IFood.ClassName when getting type, so we need to split by . and take the last part
                string name = item.GetType().ToString().Split('.').Last();

                if (Kitchen.HasIngredients(name))
                {
                    var dish = Kitchen.CookDish(name);
                    Courier.Bag.Add(dish);
                }
                else
                {
                    logger.Error($"Cannot serve order {order.Id}. Missing ingredients for {name}.");
                    throw new InvalidOperationException($"Cannot serve order. Missing ingredients for {name}.");
                }
            }

            order.UpdateStatus(OrderStatus.Ready);
            logger.Info($"Order with id {order.Id} served successfully.");
        }
        else
        {
            logger.Warn($"Cannot serve order with id {order.Id}. Either order or courier is missing.");
        }
    }

    public void StartDelivering()
    {
        courier.Status = "Delivering";
        order.UpdateStatus(OrderStatus.Delivering);
        logger.Info($"Starting delivery for order with id {order.Id} by courier {courier.Id}. Order status is now {order.Status}.");
    }

    public void DeliverOrder()
    {
        order.UpdateStatus(OrderStatus.Delivered);
        courier.Status = "Available";
        logger.Info($"Order with id {order.Id} delivered successfully. Courier {courier.Id} is now available.");
        courier = null!;
        order = null!;
    }

    public void AssignCourier(ICourier courier)
    {
        this.courier = courier;
        this.courier.Status = "On order";
        order.AssignCourier(Courier.Id);
        logger.Info($"Courier {courier.Id} assigned to order {order.Id} successfully.");
    }

    public bool HasCourier() => courier != null;
    public bool HasOrder() => order != null;
}
