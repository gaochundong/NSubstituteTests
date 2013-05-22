using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class SettingReturnValue
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_SettingReturnValue_ReturnsValueWithSpecifiedArguments()
    {
      var calculator = Substitute.For<ICalculator>();
      calculator.Add(1, 2).Returns(3);
      Assert.AreEqual(calculator.Add(1, 2), 3);
    }

    [TestMethod]
    public void Test_SettingReturnValue_ReturnsDefaultValueWithDifferentArguments()
    {
      var calculator = Substitute.For<ICalculator>();

      // 设置调用返回值为3
      calculator.Add(1, 2).Returns(3);

      Assert.AreEqual(calculator.Add(1, 2), 3);
      Assert.AreEqual(calculator.Add(1, 2), 3);

      // 当使用不同参数调用时,返回值不是3
      Assert.AreNotEqual(calculator.Add(3, 6), 3);
    }

    [TestMethod]
    public void Test_SettingReturnValue_ReturnsValueFromProperty()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Mode.Returns("DEC");
      Assert.AreEqual(calculator.Mode, "DEC");

      calculator.Mode = "HEX";
      Assert.AreEqual(calculator.Mode, "HEX");
    }
  }
}
