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

        if (!HasOrder())
        {
            throw new InvalidDataException("No active order to serve.");
        }

        // calculate all requirements first without cooking
        var requiredIngredients = new Dictionary<string, int>();
        foreach (var item in order.Items)
        {
            string name = item.GetType().Name;
            if (!requiredIngredients.ContainsKey(name)) requiredIngredients[name] = 0;
            requiredIngredients[name]++;
        }

        // check requirements against kitchen inventory
        var kitchenInventory = Kitchen.GetInventory();
        bool missingIngredients = false;

        foreach (var req in requiredIngredients)
        {
            // check if dish exist and if enough quantity is available
            if (!kitchenInventory.TryGetValue(req.Key, out int value) || value < req.Value)
            {
                missingIngredients = true;
                logger.Warn($"Missing ingredients for {req.Key}. Needed: {req.Value}, Have: {kitchenInventory.GetValueOrDefault(req.Key, 0)}");
                Console.WriteLine($"[Restaurant] ERROR: Not enough ingredients for {req.Key}!");
                break;
            }
        }

        // if missing just cancel order
        if (missingIngredients)
        {
            CancelOrder("Insufficient ingredients");
            return;
        }

        // if not missing cook the dishes
        if (!Kitchen.HasFreeCooks())
        {
            throw new InvalidOperationException("[Restaurant] Kitchen is too busy! Try again later.");
        }

        Console.WriteLine("[Restaurant] Ingredients verified. Cooking started...");
        try
        {
            foreach (var item in order.Items)
            {
                string name = item.GetType().Name;
                var dish = Kitchen.CookDish(name);
                servedDishes.Add(dish);
            }

            order.UpdateStatus(OrderStatus.Ready);
            logger.Info($"Order {order.Id} is cooked and Ready.");
        }
        catch (Exception ex)
        {
            logger.Error($"Cooking error: {ex.Message}");
            CancelOrder($"Cooking error: {ex.Message}");
        }
    }

    private void CancelOrder(string reason)
    {
        Console.WriteLine($"[Restaurant] CANCELLING Order #{order.Id}: {reason}");
        order.UpdateStatus(OrderStatus.Cancelled);

        if (courier != null)
        {
            Console.WriteLine($"[Restaurant] Releasing Courier #{courier.Id}...");
            courier.Status = "Available";
            this.courier = null!;
        }

        servedDishes.Clear();
        order = null!;

        logger.Info($"Order cancelled: {reason}");
        throw new InvalidOperationException($"Order cancelled: {reason}");
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
