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
    private readonly List<IFood> servedDishes = [];

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
        if (HasOrder())
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
                    servedDishes.Add(dish);
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
        if (HasOrder() && HasCourier())
        {
            for (int i = 0; i < servedDishes.Count; i++)
            {
                courier.Bag.Add(servedDishes[i]);
            }
            servedDishes.Clear();

            courier.Status = "Delivering";
            order.UpdateStatus(OrderStatus.Delivering);
            logger.Info($"Starting delivery for order with id {order.Id} by courier {courier.Id}. Order status is now {order.Status}.");
        }
    }
    public void DeliverOrder()
    {
        if (order.Status == OrderStatus.Delivering)
        {
            logger.Info($"Delivering order with id {order.Id} by courier {courier.Id}.");

            order.UpdateStatus(OrderStatus.Delivered);
            courier.Status = "Available";

            logger.Info($"Order with id {order.Id} delivered successfully. Courier {courier.Id} is now available.");
            courier = null!;
            order = null!;
        }
        else
        {
            logger.Warn($"Cannot deliver order with id {order.Id}. Order status is {order.Status}, expected {OrderStatus.Delivering}.");
        }
    }

    public void AssignCourier(ICourier courier)
    {
        if (!HasOrder())
        {
            logger.Warn($"Cannot assign courier {courier.Id}. No order to assign.");
            return;
        }

        if (courier.Status == "Available" && this.courier == null)
        {
            try
            {
                order.AssignCourier(courier.Id);

                this.courier = courier;
                this.courier.Status = "On order";
                logger.Info($"Courier {courier.Id} assigned to order {order.Id} successfully.");
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to assign courier: {ex.Message}");
            }
        }
        else
        {
            logger.Warn($"Cannot assign courier {courier.Id}. Courier status is {courier.Status}.");
            return;
        }
    }

    public bool HasCourier() => courier != null;
    public bool HasOrder() => order != null;
}
