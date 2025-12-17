using food_delivery.Food;

namespace food_delivery;

public class CreateOrderCommand : ICommand
{
    private readonly IRestaurant _restaurant;
    public string Name => "Create New Order";

    public CreateOrderCommand(IRestaurant restaurant)
    {
        _restaurant = restaurant;
    }

    public void Execute()
    {
        if (_restaurant.HasOrder())
        {
            Console.WriteLine("Restaurant is busy! Finish the current order first.");
            return;
        }

        Console.WriteLine("\n--- Creating Order ---");
        var items = new List<IFood>();

        while (true)
        {
            Console.WriteLine("Menu: 1. PizzaMeatLovers (250), 2. BeefBurger (180), 3. Carbonara (200)");
            Console.Write("Enter number to add (or 0 to finish): ");
            string input = Console.ReadLine()!;

            if (input == "0") break;

            IFood item = input switch
            {
                "1" => new PizzaMeatLovers(),
                "2" => new BeefBurger(),
                "3" => new Carbonara(),
                "4" => new OnionSoup(),
                "5" => new TomYumSoup(),
                "6" => new CaesarSalad(),
                "7" => new PizzaHawaiian(),
                "8" => new PizzaDiablo(),
                "9" => new PhiladelphiaRoll(),
                "10" => new CheeseSoup(),
                _ => null!
            };

            if (item != null) items.Add(item);
            else Console.WriteLine("Unknown item.");
        }

        if (items.Count == 0)
        {
            Console.WriteLine("Order cancelled (empty).");
            return;
        }

        try
        {
            var order = new OrderBuilder()
                .WithId(new Random().Next(1000, 9999))
                .WithCustomerId(101)
                .WithRestaurant(1)
                .WithDeliveryAdress("Khreshchatyk St. 22")
                .WithCustomerNumber("+380501234567")
                .WithItems(items)
                .Build();

            _restaurant.TakeOrder(order);
            Console.WriteLine($"SUCCESS: Order #{order.Id} created. Status: {order.Status}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAILED to create order: {ex.Message}");
        }
    }
}
public class ServeOrderCommand : ICommand
{
    private readonly IRestaurant _restaurant;
    public string Name => "Cook & Serve Order";

    public ServeOrderCommand(IRestaurant restaurant) => _restaurant = restaurant;

    public void Execute()
    {
        try
        {
            Console.WriteLine("Kitchen is preparing the order...");
            _restaurant.ServeOrder();
            Console.WriteLine("SUCCESS: Order is cooked and Ready.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"KITCHEN ERROR: {ex.Message}");
        }
    }
}

public class AssignCourierCommand : ICommand
{
    private readonly IRestaurant _restaurant;
    public string Name => "Assign Courier";

    public AssignCourierCommand(IRestaurant restaurant) => _restaurant = restaurant;

    public void Execute()
    {
        var courier = new Courier(new Random().Next(10, 99));

        Console.WriteLine($"Attempting to assign Courier #{courier.Id}...");
        _restaurant.AssignCourier(courier);

        if (_restaurant.HasCourier())
        {
            if (_restaurant.Courier.Id == courier.Id)
            {
                Console.WriteLine("SUCCESS: Courier assigned.");
            }
        }
        else
            Console.WriteLine("FAILED: Courier was rejected (Order might not be Ready).");
    }
}
