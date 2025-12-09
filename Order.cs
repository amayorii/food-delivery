using System;

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
    // order
    private readonly int id;
    private OrderStatus status;
    private readonly DateTime createdAt;
    private readonly DateTime updatedAt;

    // customer
    private readonly int customerId;
    private readonly string deliveryAddress;
    private readonly string customerNumber;

    // performers
    private readonly int restaurantId;
    private int courierId = 0;

    readonly List<IFood> items = [];

    private double totalPrice;
    private double deliveryPrice;

    public int Id => id;

    public OrderStatus Status { get => status; set => status = value; }

    public DateTime CreatedAt => createdAt;
    public DateTime UpdatedAt => updatedAt;

    public int CustomerId => customerId;

    public string DeliveryAddress => deliveryAddress;

    public string CustomerNumber => customerNumber;

    public int RestaurantId => restaurantId;

    public int CourierId { get => courierId; set => courierId = value; }

    public List<IFood> Items => items;

    public double TotalPrice { get => CalculateTotalPrice(); set => totalPrice = value; }
    public double DeliveryPrice { get => CalculateDeliveryPrice(); set => deliveryPrice = value; }

    public Order(int id, int restaurantId, List<IFood> items, string deliveryAddress, int customerId, string customerNumber)
    {
        this.id = id;
        this.customerId = customerId;
        this.deliveryAddress = deliveryAddress;
        this.customerNumber = customerNumber;
        this.restaurantId = restaurantId;
        this.items = items;
        Status = OrderStatus.Created;
        createdAt = DateTime.Now;
        CalculateTotalPrice();
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Cancelled) return;
        Status = newStatus;
    }

    public void AssignCourier(int courierId)
    {
        if (string.IsNullOrWhiteSpace(DeliveryAddress) || Status != OrderStatus.Ready) return;
        CourierId = courierId;
    }

    public double CalculateItemsPrice()
    {
        double price = 0;
        foreach (var item in Items)
            price += item.Cost;

        return price;
    }

    public double CalculateDeliveryPrice()
    {
        double price = CalculateItemsPrice();

        if (price < 200)
        {
            Status = OrderStatus.Cancelled;
            return 0;
        }

        switch (price)
        {
            case >= 500:
                deliveryPrice = 0;
                break;
            case >= 350 and < 500:
                deliveryPrice = 50;
                break;
            case >= 200 and < 350:
                deliveryPrice = 75;
                break;
            default:
                break;
        }

        return deliveryPrice;
    }

    public double CalculateTotalPrice() => totalPrice = CalculateItemsPrice() + CalculateDeliveryPrice();

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