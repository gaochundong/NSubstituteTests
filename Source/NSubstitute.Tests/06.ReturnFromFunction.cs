using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ReturnFromFunction
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_ReturnFromFunction_ReturnSum()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator
        .Add(Arg.Any<int>(), Arg.Any<int>())
        .Returns(x => (int)x[0] + (int)x[1]);

      Assert.AreEqual(calculator.Add(1, 1), 2);
      Assert.AreEqual(calculator.Add(20, 30), 50);
      Assert.AreEqual(calculator.Add(-73, 9348), 9275);
    }

    public interface IFoo
    {
      string Bar(int a, string b);
    }

    [TestMethod]
    public void Test_ReturnFromFunction_CallInfo()
    {
      var foo = Substitute.For<IFoo>();
      foo.Bar(0, "").ReturnsForAnyArgs(x => "Hello " + x.Arg<string>());
      Assert.AreEqual("Hello World", foo.Bar(1, "World"));
    }

    [TestMethod]
    public void Test_ReturnFromFunction_GetCallbackWhenever()
    {
      var calculator = Substitute.For<ICalculator>();

      var counter = 0;
      calculator
        .Add(0, 0)
        .ReturnsForAnyArgs(x =>
        {
          counter++;
          return 0;
        });

      calculator.Add(7, 3);
      calculator.Add(2, 2);
      calculator.Add(11, -3);
      Assert.AreEqual(counter, 3);
    }

    [TestMethod]
    public void Test_ReturnFromFunction_UseAndDoesAfterReturns()
    {
      var calculator = Substitute.For<ICalculator>();

      var counter = 0;
      calculator
        .Add(0, 0)
        .ReturnsForAnyArgs(x => 0)
        .AndDoes(x => counter++);

      calculator.Add(7, 3);
      calculator.Add(2, 2);
      Assert.AreEqual(counter, 2);
    }
  }
}
