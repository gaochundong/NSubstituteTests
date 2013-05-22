using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class CallbacksWhenDo
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_CallbacksWhenDo_PassFunctionsToReturns()
    {
      var calculator = Substitute.For<ICalculator>();

      var counter = 0;
      calculator
        .Add(0, 0)
        .ReturnsForAnyArgs(x => 0)
        .AndDoes(x => counter++);

      calculator.Add(7, 3);
      calculator.Add(2, 2);
      calculator.Add(11, -3);
      Assert.AreEqual(counter, 3);
    }

    public interface IFoo
    {
      void SayHello(string to);
    }

    [TestMethod]
    public void Test_CallbacksWhenDo_UseWhenDo()
    {
      var counter = 0;
      var foo = Substitute.For<IFoo>();

      foo.When(x => x.SayHello("World"))
        .Do(x => counter++);

      foo.SayHello("World");
      foo.SayHello("World");
      Assert.AreEqual(2, counter);
    }

    [TestMethod]
    public void Test_CallbacksWhenDo_UseWhenDoOnNonVoid()
    {
      var calculator = Substitute.For<ICalculator>();

      var counter = 0;
      calculator.Add(1, 2).Returns(3);
      calculator
        .When(x => x.Add(Arg.Any<int>(), Arg.Any<int>()))
        .Do(x => counter++);

      var result = calculator.Add(1, 2);
      Assert.AreEqual(3, result);
      Assert.AreEqual(1, counter);
    }
  }
}
