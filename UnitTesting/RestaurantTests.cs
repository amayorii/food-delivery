using food_delivery;
using food_delivery.Food;
using Moq;
using NUnit.Framework;

namespace UnitTesting;

public class RestaurantTests
{
    private Mock<IKitchen> kitchenMock;
    private Mock<ICourier> courierMock;
    private Restaurant restaurant;

    [SetUp]
    public void Setup()
    {
        kitchenMock = new Mock<IKitchen>();
        courierMock = new Mock<ICourier>();

        var inventory = new Dictionary<string, int>
        {
            { "PizzaMeatLovers", 5 },
            { "BeefBurger", 5 },
            { "Carbonara", 5 },
            { "PizzaDiablo", 0 },
            { "PhiladelphiaRoll", 5 },

        };
        kitchenMock.Setup(k => k.GetInventory()).Returns(inventory);

        courierMock.Setup(c => c.Bag).Returns(new List<IFood>());
        courierMock.Setup(c => c.Status).Returns("Available");
        courierMock.Setup(c => c.Id).Returns(50);

        restaurant = new Restaurant(kitchenMock.Object);
    }

    [Test]
    public void ServeOrder_KitchenHasIngredients_FoodInBag()
    {
        kitchenMock.Setup(k => k.HasFreeCooks()).Returns(true);

        kitchenMock.Setup(k => k.CookDish("Carbonara")).Returns(new Carbonara());

        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(new List<IFood> { new Carbonara() })
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        restaurant.TakeOrder(order);

        restaurant.ServeOrder();

        restaurant.AssignCourier(courierMock.Object);

        restaurant.StartDelivering();

        Assert.That(restaurant.Courier.Bag, Has.Some.InstanceOf<Carbonara>());
    }

    [Test]
    public void ServeOrder_KitchenHasNoIngredients_ThrowsExc()
    {
        kitchenMock.Setup(k => k.HasFreeCooks()).Returns(true);

        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(new List<IFood> { new PizzaDiablo() })
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        restaurant.TakeOrder(order);

        var ex = Assert.Throws<InvalidOperationException>(() => restaurant.ServeOrder());

        Assert.That(ex.Message, Does.Contain("Order cancelled: Insufficient ingredients"));
    }

    [Test]
    public void AssignCourier_RestaurantAlreadyHasCourier_DoesntReassign()
    {
        kitchenMock.Setup(k => k.HasFreeCooks()).Returns(true);

        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(new List<IFood> { new PhiladelphiaRoll() })
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        restaurant.TakeOrder(order);

        var firstCourier = new Mock<ICourier>();
        firstCourier.Setup(c => c.Status).Returns("Available");
        firstCourier.Setup(c => c.Id).Returns(1);

        var secondCourier = new Mock<ICourier>();
        secondCourier.Setup(c => c.Status).Returns("Available");
        secondCourier.Setup(c => c.Id).Returns(2);

        restaurant.ServeOrder();

        restaurant.AssignCourier(firstCourier.Object);
        restaurant.AssignCourier(secondCourier.Object);

        Assert.That(restaurant.Courier.Id, Is.EqualTo(1));
    }

    [Test]
    public void StartDelivering_Invoke_UpdatesStatuses()
    {
        kitchenMock.Setup(k => k.HasFreeCooks()).Returns(true);

        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(new List<IFood> { new PhiladelphiaRoll() })
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        courierMock.SetupProperty(c => c.Status, "Available");

        restaurant.TakeOrder(order);
        restaurant.ServeOrder();
        restaurant.AssignCourier(courierMock.Object);

        restaurant.StartDelivering();

        Assert.Multiple(() =>
        {
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Delivering));
            Assert.That(restaurant.Courier.Status, Is.EqualTo("Delivering"));
        });
    }

    [Test]
    public void ServeOrder_NoFreeCooks_ThrowsExc()
    {
        kitchenMock.Setup(k => k.HasFreeCooks()).Returns(false);

        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(new List<IFood> { new Carbonara() })
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        restaurant.TakeOrder(order);

        var ex = Assert.Throws<InvalidOperationException>(restaurant.ServeOrder);
        Assert.That(ex.Message, Is.EqualTo("[Restaurant] Kitchen is too busy! Try again later."));
    }

    [Test]
    public void ServeOrder_ThereAreFreeCooks_ThrowsExc()
    {
        kitchenMock.Setup(k => k.HasFreeCooks()).Returns(true);

        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(new List<IFood> { new Carbonara() })
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        restaurant.TakeOrder(order);
        restaurant.AssignCourier(courierMock.Object);

        Assert.That(restaurant.ServeOrder, Throws.Nothing);
    }
}