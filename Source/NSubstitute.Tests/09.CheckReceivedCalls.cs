using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class CheckReceivedCalls
  {
    public interface ICommand
    {
      void Execute();
      event EventHandler Executed;
    }

    public class SomethingThatNeedsACommand
    {
      ICommand command;
      public SomethingThatNeedsACommand(ICommand command)
      {
        this.command = command;
      }
      public void DoSomething() { command.Execute(); }
      public void DontDoAnything() { }
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_CallReceived()
    {
      //Arrange
      var command = Substitute.For<ICommand>();
      var something = new SomethingThatNeedsACommand(command);

      //Act
      something.DoSomething();

      //Assert
      command.Received().Execute();
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_CallDidNotReceived()
    {
      //Arrange
      var command = Substitute.For<ICommand>();
      var something = new SomethingThatNeedsACommand(command);

      //Act
      something.DontDoAnything();

      //Assert
      command.DidNotReceive().Execute();
    }

    public class CommandRepeater
    {
      ICommand command;
      int numberOfTimesToCall;
      public CommandRepeater(ICommand command, int numberOfTimesToCall)
      {
        this.command = command;
        this.numberOfTimesToCall = numberOfTimesToCall;
      }

      public void Execute()
      {
        for (var i = 0; i < numberOfTimesToCall; i++) command.Execute();
      }
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_CallReceivedNumberOfSpecifiedTimes()
    {
      // Arrange
      var command = Substitute.For<ICommand>();
      var repeater = new CommandRepeater(command, 3);

      // Act
      repeater.Execute();

      // Assert
      // 如果仅接收到2次或者4次，这里会失败。
      command.Received(3).Execute();
    }

    public interface ICalculator
    {
      int Add(int a, int b);
      int Subtract(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_CallReceivedWithSpecificArguments()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Add(1, 2);
      calculator.Add(-100, 100);

      // 检查接收到了第一个参数为任意值，第二个参数为2的调用
      calculator.Received().Add(Arg.Any<int>(), 2);
      // 检查接收到了第一个参数小于0，第二个参数为100的调用
      calculator.Received().Add(Arg.Is<int>(x => x < 0), 100);
      // 检查未接收到第一个参数为任意值，第二个参数大于等于500的调用
      calculator
        .DidNotReceive()
        .Add(Arg.Any<int>(), Arg.Is<int>(x => x >= 500));
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_IgnoringArguments()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Add(1, 3);

      calculator.ReceivedWithAnyArgs().Add(1, 1);
      calculator.DidNotReceiveWithAnyArgs().Subtract(0, 0);
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_CheckingCallsToPropeties()
    {
      var calculator = Substitute.For<ICalculator>();

      var mode = calculator.Mode;
      calculator.Mode = "TEST";

      // 检查接收到了对属性 getter 的调用
      // 这里需要使用临时变量以通过编译
      var temp = calculator.Received().Mode;

      // 检查接收到了对属性 setter 的调用，参数为"TEST"
      calculator.Received().Mode = "TEST";
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_CheckingCallsToIndexers()
    {
      var dictionary = Substitute.For<IDictionary<string, int>>();
      dictionary["test"] = 1;

      dictionary.Received()["test"] = 1;
      dictionary.Received()["test"] = Arg.Is<int>(x => x < 5);
    }

    public class CommandWatcher
    {
      ICommand command;
      public CommandWatcher(ICommand command)
      {
        this.command = command;
        this.command.Executed += OnExecuted;
      }
      public bool DidStuff { get; private set; }
      public void OnExecuted(object o, EventArgs e) 
      { 
        DidStuff = true; 
      }
    } 

    [TestMethod]
    public void Test_CheckReceivedCalls_CheckingEventSubscriptions()
    {
      var command = Substitute.For<ICommand>();
      var watcher = new CommandWatcher(command);

      command.Executed += Raise.Event();

      Assert.IsTrue(watcher.DidStuff);
    }

    [TestMethod]
    public void Test_CheckReceivedCalls_MakeSureWatcherSubscribesToCommandExecuted()
    {
      var command = Substitute.For<ICommand>();
      var watcher = new CommandWatcher(command);

      // 不推荐这种方法。
      // 更好的办法是测试行为而不是具体实现。
      command.Received().Executed += watcher.OnExecuted;
      // 或者有可能事件处理器是不可访问的。
      command.Received().Executed += Arg.Any<EventHandler>();
    }
  }
}
