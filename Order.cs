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
        logger.Info($"Updating status from {Status} to {newStatus} for order with id: {Id}");

        if(Status == OrderStatus.Cancelled)
        {
            logger.Warn("Cannot update status of a cancelled order. Order id: " + Id);
            return;
        }
        Status = newStatus;

        logger.Info($"Status updated to {Status} for order with id: {Id}");
    }

    public double CalculateItemsPrice()
    {
        logger.Info("Calculating total price of items in the order with id: " + Id);

        double price = 0;
        foreach(var item in Items)
            price += item.Cost;

        logger.Info($"Total items price calculated: {price}. Order id: {Id}");
        return price;
    }

    public void AssignCourier(int courierId)
    {
        try
        {
            logger.Info($"Assigning courier with id {courierId} to order with id {Id}");

            ValidateOrder();
            CourierId = courierId;
        }
        catch(Exception ex)
        {
            logger.Error($"Error assigning courier to order with id {Id}: {ex.Message}");

            CourierId = 0;
            Console.WriteLine(ex.Message);
        }

    }

    public void ValidateOrder()
    {
        logger.Info("Validating order with id: " + Id);

        if(string.IsNullOrWhiteSpace(DeliveryAddress))
        {
            logger.Error("Validation failed: Delivery address is empty. Order id: " + Id);
            throw new ArgumentException("Delivery address cannot be empty.");
        }

        if(Items.Count == 0)
        {
            logger.Error("Validation failed: No items in the order. Order id: " + Id);
            throw new ArgumentException("Order must contain at least one item.");
        }

        if(CalculateItemsPrice() < 200)
        {
            logger.Error("Validation failed: Total items price is below 200. Order id: " + Id);
            throw new ArgumentException("Total items price must be at least 200.");
        }

        if(Status != OrderStatus.Ready)
        {
            logger.Error("Validation failed: Order status is not 'Ready'. Order id: " + Id);
            throw new InvalidOperationException("Can only assign courier to a ready order.");
        }

        logger.Info("Order with id " + Id + " validated successfully.");
    }

    public double CalculateDeliveryPrice(out double price)
    {
        logger.Info("Calculating delivery price for order with id: " + Id);
        price = CalculateItemsPrice();

        deliveryPrice = price switch
        {
            >= 500 => 0,
            >= 350 and < 500 => 50,
            >= 200 and < 350 => 75,
            _ => 100,
        };

        logger.Info($"Delivery price calculated: {deliveryPrice}. Order id: {Id}");
        return deliveryPrice;
    }

    public double CalculateTotalPrice()
    {
        logger.Info("Calculating total order price for order with id: " + Id);

        totalPrice = CalculateDeliveryPrice(out double price) + price;

        logger.Info($"Total order price calculated: {totalPrice}. Order id: {Id}");

        return totalPrice;
    }

    public void AddItem(IFood food)
    {
        logger.Info($"Adding item to order with id: {Id}");

        Items.Add(food);
        CalculateTotalPrice();

        logger.Info($"Item added to order with id: {Id}");
    }

    public void RemoveItem(IFood food)
    {
        logger.Info($"Removing item from order with id: {Id}");

        Items.Remove(food);
        CalculateTotalPrice();

        logger.Info($"Item removed from order with id: {Id}");
    }
}