using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ReturnForSpecificArgs
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_ReturnForSpecificArgs_UseArgumentsMatcher()
    {
      var calculator = Substitute.For<ICalculator>();

      // 当第一个参数是任意int类型的值，第二个参数是5时返回。
      calculator.Add(Arg.Any<int>(), 5).Returns(10);
      Assert.AreEqual(10, calculator.Add(123, 5));
      Assert.AreEqual(10, calculator.Add(-9, 5));
      Assert.AreNotEqual(10, calculator.Add(-9, -9));

      // 当第一个参数是1，第二个参数小于0时返回。
      calculator.Add(1, Arg.Is<int>(x => x < 0)).Returns(345);
      Assert.AreEqual(345, calculator.Add(1, -2));
      Assert.AreNotEqual(345, calculator.Add(1, 2));

      // 当两个参数都为0时返回。
      calculator.Add(Arg.Is(0), Arg.Is(0)).Returns(99);
      Assert.AreEqual(99, calculator.Add(0, 0));
    }
  }
}
