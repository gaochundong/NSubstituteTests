using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ReplaceReturnValues
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_ReplaceReturnValues_ReplaceSeveralTimes()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Mode.Returns("DEC,HEX,OCT");
      calculator.Mode.Returns(x => "???");
      calculator.Mode.Returns("HEX");
      calculator.Mode.Returns("BIN");

      Assert.AreEqual(calculator.Mode, "BIN");
    }
  }
}
