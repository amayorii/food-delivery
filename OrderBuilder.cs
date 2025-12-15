namespace food_delivery;

public class OrderBuilder
{
    private readonly Order _order = new();

    public OrderBuilder WithId(int id)
    {
        _order.Id = id;
        return this;
    }

    public OrderBuilder WithRestaurant(int restaurantId)
    {
        _order.RestaurantId = restaurantId;
        return this;
    }

    public OrderBuilder WithDeliveryAdress(string delAdress)
    {
        _order.DeliveryAddress = delAdress;
        return this;
    }

    public OrderBuilder WithCustomerNumber(string custNumber)
    {
        _order.CustomerNumber = custNumber;
        return this;
    }

    public OrderBuilder WithCustomerId(int custId)
    {
        _order.CustomerId = custId;
        return this;
    }

    public OrderBuilder WithItems(List<IFood> items)
    {
        _order.Items.AddRange(items);
        _order.CalculateTotalPrice();
        return this;
    }

    public Order Build()
    {
        if (string.IsNullOrEmpty(_order.DeliveryAddress) || string.IsNullOrEmpty(_order.CustomerNumber) || _order.RestaurantId == default)
        {
            throw new InvalidOperationException("Delivery address, customer number and restaurant must be set.");
        }

        return _order;
    }
}