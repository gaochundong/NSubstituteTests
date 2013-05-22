using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ReturnForAnyArgs
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_ReturnForAnyArgs_ReturnForAnyArgs()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Add(1, 2).ReturnsForAnyArgs(100);
      Assert.AreEqual(calculator.Add(1, 2), 100);
      Assert.AreEqual(calculator.Add(-7, 15), 100);
    }
  }
}
