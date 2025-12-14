using System;

namespace food_delivery;

public interface ICourier
{
    public int Id { get; set; }
    public string Status { get; set; }
    public List<IFood> Bag { get; set; }
}
