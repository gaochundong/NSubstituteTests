using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ActionsWithArgumentMatchers
  {
    public interface IEvents
    {
      void RaiseOrderProcessed(int orderId);
    }

    public interface ICart
    {
      int OrderId { get; set; }
    }

    public interface IOrderProcessor
    {
      void ProcessOrder(int orderId, Action<bool> orderProcessed);
    }

    public class OrderPlacedCommand
    {
      IOrderProcessor orderProcessor;
      IEvents events;
      public OrderPlacedCommand(IOrderProcessor orderProcessor, IEvents events)
      {
        this.orderProcessor = orderProcessor;
        this.events = events;
      }
      public void Execute(ICart cart)
      {
        orderProcessor.ProcessOrder(
            cart.OrderId,
            wasOk => { if (wasOk) events.RaiseOrderProcessed(cart.OrderId); }
        );
      }
    }

    [TestMethod]
    public void Test_ActionsWithArgumentMatchers_InvokingCallbacks()
    {
      // Arrange
      var cart = Substitute.For<ICart>();
      var events = Substitute.For<IEvents>();
      var processor = Substitute.For<IOrderProcessor>();
      cart.OrderId = 3;
      // 设置 processor 当处理订单ID为3时，调用回调函数，参数为true
      processor.ProcessOrder(3, Arg.Invoke(true));

      // Act
      var command = new OrderPlacedCommand(processor, events);
      command.Execute(cart);

      // Assert
      events.Received().RaiseOrderProcessed(3);
    }

    public interface ICalculator
    {
      int Multiply(int a, int b);
    }

    [TestMethod]
    public void Test_ActionsWithArgumentMatchers_PerformingActionsWithArgs()
    {
      var calculator = Substitute.For<ICalculator>();

      var argumentUsed = 0;
      calculator.Multiply(Arg.Any<int>(), Arg.Do<int>(x => argumentUsed = x));

      calculator.Multiply(123, 42);

      Assert.AreEqual(42, argumentUsed);
    }

    [TestMethod]
    public void Test_ActionsWithArgumentMatchers_PerformingActionsWithAnyArgs()
    {
      var calculator = Substitute.For<ICalculator>();

      var firstArgsBeingMultiplied = new List<int>();
      calculator.Multiply(Arg.Do<int>(x => firstArgsBeingMultiplied.Add(x)), 10);

      calculator.Multiply(2, 10);
      calculator.Multiply(5, 10);

      // 由于第二个参数不为10，所以不会被 Arg.Do 匹配
      calculator.Multiply(7, 4567); 

      CollectionAssert.AreEqual(firstArgsBeingMultiplied, new[] { 2, 5 });
    }

    [TestMethod]
    public void Test_ActionsWithArgumentMatchers_ArgActionsCallSpec()
    {
      var calculator = Substitute.For<ICalculator>();

      var numberOfCallsWhereFirstArgIsLessThan0 = 0;

      // 指定调用参数：
      // 第一个参数小于0
      // 第二个参数可以为任意的int类型值
      // 当此满足此规格时，为计数器加1。
      calculator
        .Multiply(
          Arg.Is<int>(x => x < 0),
          Arg.Do<int>(x => numberOfCallsWhereFirstArgIsLessThan0++)
        ).Returns(123);

      var results = new[] {
        calculator.Multiply(-4, 3),
        calculator.Multiply(-27, 88),
        calculator.Multiply(-7, 8),
        calculator.Multiply(123, 2) // 第一个参数大于0，所以不会被匹配
      };

      Assert.AreEqual(3, numberOfCallsWhereFirstArgIsLessThan0); // 4个调用中有3个匹配上
      CollectionAssert.AreEqual(results, new[] { 123, 123, 123, 0 }); // 最后一个未匹配
    }
  }
}
