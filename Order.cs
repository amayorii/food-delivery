using System;

namespace food_delivery;

public class Order
{
    // order
    private readonly int id;
    // TODO create OrderStatus enum and use it instead of string
    private string? status;
    private readonly DateTime createdAt;
    private DateTime updatedAt;

    // customer
    private int customerId;
    private string deliveryAddress;
    private string customerNumber;

    // performers
    private int restaurantId;
    private int courierId = 0;

    readonly List<IFood> items = [];

    private double totalPrice;
    private double deliveryPrice;

    public int Id => id;

    // TODO use enum OrderStatus
    public string? Status { get => status; set => status = value; }

    public DateTime CreatedAt => createdAt;
    public DateTime UpdatedAt { get => updatedAt; set => updatedAt = value; }

    public int CustomerId { get => customerId; set => customerId = value; }

    public string DeliveryAddress { get => deliveryAddress; set => deliveryAddress = value; }

    public string CustomerNumber { get => customerNumber; set => customerNumber = value; }

    public int RestaurantId { get => restaurantId; set => restaurantId = value; }

    public int CourierId { get => courierId; set => courierId = value; }

    public List<IFood> Items => items;

    public double TotalPrice { get => /*TODO CalculateTotalPrice() instead of -> */ totalPrice; set => totalPrice = value; }
    public double DeliveryPrice { get => /*TODO CalculateDeliveryPrice() instead of -> */ deliveryPrice; set => deliveryPrice = value; }

    public Order(int id, int restaurantId, List<IFood> items, string deliveryAddress, int customerId, string customerNumber)
    {
        this.id = id;
        this.customerId = customerId;
        this.deliveryAddress = deliveryAddress;
        this.customerNumber = customerNumber;
        this.restaurantId = restaurantId;
        this.items = items;
        Status = "Created"; // use OrderStatus enum
        createdAt = DateTime.Now;
        //CalculateTotalPrice(); uncomment when implemented
    }

    public double CalculateItemsPrice()
    {
        double price = 0;
        foreach (var item in Items)
            price += item.Cost;

        return price;
    }
}