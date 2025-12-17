namespace food_delivery;

class Program
{
    static void Main(string[] args)
    {
        IKitchen kitchen = new Kitchen(3);
        Restaurant restaurant = new Restaurant(kitchen);

        Menu mainMenu = new Menu("Food Delivery System");
        Menu kitchenMenu = new Menu("Kitchen Operations");

        // Build kitchen menu
        kitchenMenu.Add(new KitchenStatusCommand(kitchen));
        kitchenMenu.Add(new ServeOrderCommand(restaurant));

        // Build main menu
        mainMenu.Add(new CreateOrderCommand(restaurant));
        mainMenu.Add(kitchenMenu);
        mainMenu.Add(new AssignCourierCommand(restaurant));
        mainMenu.Add(new DeliverOrderCommand(restaurant));

        mainMenu.Execute();
    }
}
