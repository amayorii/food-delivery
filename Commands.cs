using food_delivery.Food;

namespace food_delivery;

public class DeliverOrderCommand : ICommand
{
    private readonly IRestaurant _restaurant;
    public string Name => "Deliver Order";

    public DeliverOrderCommand(IRestaurant restaurant) => _restaurant = restaurant;

    public void Execute()
    {
        try
        {
            _restaurant.StartDelivering();
            Console.WriteLine("Order is on the way...");

            Thread.Sleep(500);

            _restaurant.DeliverOrder();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DELIVERY ERROR: {ex.Message}");
        }
    }
}

public class KitchenStatusCommand : ICommand
{
    private readonly IKitchen _kitchen;
    public string Name => "Check Kitchen Inventory";

    public KitchenStatusCommand(IKitchen kitchen) => _kitchen = kitchen;

    public void Execute()
    {
        Console.WriteLine("\n--- Kitchen status ---");
        var inventory = _kitchen.GetInventory();

        foreach (var item in inventory)
        {
            string status = item.Value > 0 ? $"Available ({item.Value})" : "Out of stock";

            Console.WriteLine($"{item.Key}: {status}");
        }

        Console.WriteLine($"Free cooks: {_kitchen.FreeCooks}");
    }
}
