using System;
using NLog;

namespace food_delivery;

public enum OrderStatus
{
    Created,
    InProgress,
    Ready,
    Delivering,
    Delivered,
    Cancelled
}
public class Order
{
    // LOGGER
    private readonly static Logger logger = LogManager.GetLogger("myAppLoggerRule");

    // order
    private int id;
    private OrderStatus status;
    private readonly DateTime createdAt;
    private DateTime updatedAt;

    // customer
    private int customerId;
    private string? deliveryAddress;
    private string? customerNumber;

    // performers
    private int restaurantId;
    private int courierId = 0;

    readonly List<IFood> items = [];

    private double totalPrice;
    private double deliveryPrice;

    public int Id { get => id; set => id = value; }

    public OrderStatus Status { get => status; set => status = value; }

    public DateTime CreatedAt => createdAt;
    public DateTime UpdatedAt { get => updatedAt; set => updatedAt = value; }

    public int CustomerId { get => customerId; set => customerId = value; }

    public string? DeliveryAddress { get => deliveryAddress; set => deliveryAddress = value; }

    public string? CustomerNumber { get => customerNumber; set => customerNumber = value; }

    public int RestaurantId { get => restaurantId; set => restaurantId = value; }

    public int CourierId { get => courierId; set => courierId = value; }

    public List<IFood> Items => items;

    public double TotalPrice { get => totalPrice; set => totalPrice = value; }
    public double DeliveryPrice { get => deliveryPrice; set => deliveryPrice = value; }

    public Order()
    {
        Status = OrderStatus.Created;
        createdAt = DateTime.Now;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Cancelled) return;
        Status = newStatus;
    }

    public double CalculateItemsPrice()
    {
        double price = 0;
        foreach (var item in Items)
            price += item.Cost;

        return price;
    }

    public void AssignCourier(int courierId)
    {
        try
        {
            ValidateOrder();
            CourierId = courierId;
        }
        catch (Exception ex)
        {
            CourierId = 0;
            Console.WriteLine(ex.Message);
        }

    }

    public void ValidateOrder()
    {
        if (string.IsNullOrWhiteSpace(DeliveryAddress))
            throw new ArgumentException("Delivery address cannot be empty.");

        if (Items.Count == 0)
            throw new ArgumentException("Order must contain at least one item.");

        if (CalculateItemsPrice() < 200)
            throw new ArgumentException("Total items price must be at least 200.");

        if (Status != OrderStatus.Ready)
            throw new InvalidOperationException("Can only assign courier to a ready order.");
    }

    public double CalculateDeliveryPrice(out double price)
    {
        price = CalculateItemsPrice();

        deliveryPrice = price switch
        {
            >= 500 => 0,
            >= 350 and < 500 => 50,
            >= 200 and < 350 => 75,
            _ => 100,
        };

        return deliveryPrice;
    }

    public double CalculateTotalPrice()
    {
        totalPrice = CalculateDeliveryPrice(out double price) + price;
        return totalPrice;
    }

    public void AddItem(IFood food)
    {
        Items.Add(food);
        CalculateTotalPrice();
    }

    public void RemoveItem(IFood food)
    {
        Items.Remove(food);
        CalculateTotalPrice();
    }
}