using food_delivery.Food;

namespace food_delivery;

public class Kitchen : IKitchen
{
    private readonly Dictionary<string, int> inventory = [];

    private readonly int _totalCooks;
    private int _busyCooks;
    public int FreeCooks => _totalCooks - _busyCooks;

    public Kitchen(int initialCooks)
    {
        _totalCooks = initialCooks;
        _busyCooks = 0;

        // initialize with some starter ingredients
        Restock("BeefBurger", 5);
        Restock("CaesarSalad", 4);
        Restock("Carbonara", 3);
        Restock("CheeseSoup", 2);
        Restock("OnionSoup", 2);
        Restock("PhiladelphiaRoll", 3);
        Restock("PizzaDiablo", 1);
        Restock("PizzaHawaiian", 0);
        Restock("PizzaMeatLovers", 2);
        Restock("TomYumSoup", 2);
    }

    public IFood CookDish(string name)
    {
        if (!HasFreeCooks())
        {
            throw new InvalidOperationException("Kitchen is overwhelmed! No free cooks.");
        }

        if (!HasIngredients(name))
        {
            throw new InvalidOperationException($"Not enough ingredients to cook {name}.");
        }

        _busyCooks++;
        inventory[name]--;

        Console.WriteLine($"[Kitchen] Chef is cooking {name}... (Remaining supply: {inventory[name]})");

        IFood dish = CreateFoodInstance(name);

        _busyCooks--;

        return dish;
    }

    // factory method to create the concrete food objects
    private static IFood CreateFoodInstance(string name)
    {
        return name switch
        {
            "BeefBurger" => new BeefBurger(),
            "CaesarSalad" => new CaesarSalad(),
            "Carbonara" => new Carbonara(),
            "CheeseSoup" => new CheeseSoup(),
            "OnionSoup" => new OnionSoup(),
            "PhiladelphiaRoll" => new PhiladelphiaRoll(),
            "PizzaDiablo" => new PizzaDiablo(),
            "PizzaHawaiian" => new PizzaHawaiian(),
            "PizzaMeatLovers" => new PizzaMeatLovers(),
            "TomYumSoup" => new TomYumSoup(),
            _ => throw new ArgumentException($"Unknown dish: {name}")
        };
    }

    public bool HasIngredients(string name)
    {
        return inventory.ContainsKey(name) && inventory[name] > 0;
    }

    public bool HasFreeCooks()
    {
        return _busyCooks < _totalCooks;
    }

    public void Restock(string dishName, int quantity)
    {
        if (!inventory.ContainsKey(dishName))
        {
            inventory[dishName] = 0;
        }
        inventory[dishName] += quantity;
    }

    public Dictionary<string, int> GetInventory()
    {
        return new Dictionary<string, int>(inventory);
    }
}