using System;

namespace food_delivery;

public interface IKitchen
{
    public IFood CookDish(string name);
    public bool HasIngredients(string name);
    public bool HasFreeCooks();
    public void Restock(string name, int quantity);
    public Dictionary<string, int> GetInventory();
    int FreeCooks { get; }
}
