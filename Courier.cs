namespace food_delivery;

public class Courier : ICourier
{
    public int Id { get; set; }
    public string Status { get; set; }
    public List<IFood> Bag { get; set; }

    public Courier(int id)
    {
        Id = id;
        Status = "Available";
        Bag = new List<IFood>();
    }
}