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
        if (HasOrder() && HasCourier() && order.Status == OrderStatus.Ready)
        {
            // move food to courier
            if (servedDishes.Count > 0)
            {
                courier.Bag.AddRange(servedDishes);
                servedDishes.Clear();
            }

            courier.Status = "Delivering";
            order.UpdateStatus(OrderStatus.Delivering);
            logger.Info($"Order {order.Id} delivery started.");
            Console.WriteLine($"Courier #{courier.Id} is delivering Order #{order.Id}.");
        }
        else
        {
            logger.Warn("Failed to start delivery due to invalid state.");
            throw new InvalidOperationException("Cannot start delivery. (Check if Order is Ready and Courier is Assigned)");
        }
    }

    public void DeliverOrder()
    {
        if (HasOrder() && HasCourier() && order.Status == OrderStatus.Delivering)
        {
            logger.Info($"Delivering order with id {order.Id} by courier {courier.Id}.");

            order.UpdateStatus(OrderStatus.Delivered);

            courier.Status = "Available";
            courier.Bag.Clear();

            logger.Info($"Order with id {order.Id} delivered successfully. Courier {courier.Id} is now available.");
            Console.WriteLine($"SUCCESS: Order #{order.Id} delivered!");

            courier = null!;
            order = null!;
        }
        else
        {
            logger.Warn($"Cannot deliver order with id {order!.Id}. Order status is {order.Status}, expected {OrderStatus.Delivering}.");
        }
    }

    public void AssignCourier(ICourier courier)
    {
        if (!HasOrder())
        {
            logger.Warn("Cannot assign courier because no order provided");
            Console.WriteLine("No order to assign to.");
            return;
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            Console.WriteLine("Cannot assign courier to a Cancelled order.");
            logger.Warn("Cannot assign courier to a Cancelled order");
            return;
        }

        if (HasCourier())
        {
            Console.WriteLine("A courier is already assigned.");
            logger.Warn("A courier is already assigned");
            return;
        }

        if (courier.Status == "Available")
        {
            try
            {
                if (order.Status != OrderStatus.Ready)
                {
                    logger.Warn("Order not ready");
                    Console.WriteLine("Order not ready.");
                    return;
                }

                order.AssignCourier(courier.Id);

                this.courier = courier;
                this.courier.Status = "OnOrder";
                Console.WriteLine($"Courier #{courier.Id} assigned.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Assignment failed: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Courier #{courier.Id} is busy.");
        }
    }

    public bool HasCourier() => courier != null;
    public bool HasOrder() => order != null;
}