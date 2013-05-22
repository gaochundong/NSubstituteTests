using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class MultipleReturnValues
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_MultipleReturnValues_ReturnMultipleValues()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Mode.Returns("DEC", "HEX", "BIN");
      Assert.AreEqual("DEC", calculator.Mode);
      Assert.AreEqual("HEX", calculator.Mode);
      Assert.AreEqual("BIN", calculator.Mode);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public void Test_MultipleReturnValues_UsingCallbacks()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Mode.Returns(x => "DEC", x => "HEX", x => { throw new Exception(); });
      Assert.AreEqual("DEC", calculator.Mode);
      Assert.AreEqual("HEX", calculator.Mode);
      var result = calculator.Mode;
    }
  }
}
