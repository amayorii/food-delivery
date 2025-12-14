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
    [Test]
    public void CalculateTotalPrice_ItemsPriceAbove500_DeliveryPriceIsZero()
    {
        List<IFood> items = new() { new Carbonara(), new PhiladelphiaRoll(), new TomYumSoup(), new PizzaDiablo() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.Multiple(() =>
        {
            Assert.That(order.DeliveryPrice, Is.EqualTo(0));
            Assert.That(order.TotalPrice, Is.EqualTo(119.99 + 129.99 + 149.99 + 149.99 + order.DeliveryPrice));
        });
    }

    [Test]
    public void CalculateTotalPrice_ItemsPriceBetween350And500_DeliveryPriceIs50ButTotalDoesntEqual()
    {
        List<IFood> items = new() { new Carbonara(), new TomYumSoup(), new PizzaDiablo() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.Multiple(() =>
        {
            Assert.That(order.DeliveryPrice, Is.EqualTo(50));
            Assert.That(order.TotalPrice, Is.Not.EqualTo(119.99 + 149.99 + 149.99 + 70));
        });
    }

    [Test]
    public void AddItem_PriceChanged_TotalEquals()
    {
        List<IFood> items = new() { new OnionSoup() };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();
        PizzaMeatLovers pizza = new PizzaMeatLovers();
        order.AddItem(pizza);

        Assert.That(order.TotalPrice, Is.EqualTo(154.99 + 99.99 + order.DeliveryPrice));
    }

    [Test]
    public void RemoveItem_TotalBelow200_DeliveryPriceEquals100()
    {
        PizzaDiablo item1 = new();
        PhiladelphiaRoll item2 = new();

        List<IFood> items = new() { item1, item2 };
        Order order = new OrderBuilder().WithId(1)
                                        .WithRestaurant(1)
                                        .WithItems(items)
                                        .WithDeliveryAdress("Test Address")
                                        .WithCustomerId(1)
                                        .WithCustomerNumber("+380978312233")
                                        .Build();

        Assert.That(order.DeliveryPrice, Is.EqualTo(75));
        order.RemoveItem(item2);
        Assert.That(order.DeliveryPrice, Is.EqualTo(100));
    }
}
