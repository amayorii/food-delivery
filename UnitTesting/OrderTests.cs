using food_delivery;
using food_delivery.Food;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace UnitTesting;

public class Tests
{
    [Test]
    public void CalculateItemsPrice_319and97_Equals()
    {
        List<IFood> items = new() { new CaesarSalad(), new BeefBurger(), new PizzaHawaiian() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.That(order.CalculateItemsPrice(), Is.EqualTo(319.97));
    }

    [Test]
    public void CalculateItemsPrice_319and97_DoesntEqual()
    {
        List<IFood> items = new() { new CaesarSalad(), new BeefBurger(), new PizzaHawaiian() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.That(order.CalculateItemsPrice(), Is.Not.EqualTo(320.00));
    }

    [Test]
    public void Ctor_WithoutItems_ListEmpty()
    {
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.That(order.Items, Is.Empty);
    }

    [Test]
    public void Ctor_Status_Created()
    {
        List<IFood> items = new() { new BeefBurger(), new PhiladelphiaRoll(), new PizzaDiablo() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.That(order.Status, Is.EqualTo(OrderStatus.Created));
    }
    [Test]
    public void UpdateStatus_FromCreatedToInProgress_Changes()
    {
        List<IFood> items = new() { new Carbonara(), new PhiladelphiaRoll(), new PizzaDiablo() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        order.UpdateStatus(OrderStatus.InProgress);

        Assert.That(order.Status, Is.EqualTo(OrderStatus.InProgress));
    }

    [Test]
    public void UpdateStatus_FromCancelledToInProgress_DoesntChange()
    {
        List<IFood> items = new() { new Carbonara(), new PhiladelphiaRoll(), new PizzaDiablo() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        order.UpdateStatus(OrderStatus.Cancelled);
        order.UpdateStatus(OrderStatus.InProgress);

        Assert.That(order.Status, Is.Not.EqualTo(OrderStatus.InProgress));
    }

    [Test]
    public void AssignCourier_ItemsPriceLessThan200_DoesntSetCourier()
    {
        List<IFood> items = new() { new BeefBurger() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        order.AssignCourier(42); 
        Assert.That(order.CourierId, Is.EqualTo(0));
    }

    [Test]
    public void ValidateOrder_ItemsPriceBelow200_ThrowsExc()
    {
        List<IFood> items = new() { new OnionSoup() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
         Assert.That(() => order.ValidateOrder(), Throws.Exception.TypeOf<ArgumentException>());
    }
}
