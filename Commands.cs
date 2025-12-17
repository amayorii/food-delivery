using food_delivery.Food;

namespace food_delivery;
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