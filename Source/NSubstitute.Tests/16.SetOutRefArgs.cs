using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class SetOutRefArgs
  {
    public interface ILookup
    {
      bool TryLookup(string key, out string value);
    }

    [TestMethod]
    public void Test_SetOutRefArgs_SetOutArg()
    {
      // Arrange
      var value = "";
      var lookup = Substitute.For<ILookup>();
      lookup
        .TryLookup("hello", out value)
        .Returns(x =>
        {
          x[1] = "world!";
          return true;
        });

      // Act
      var result = lookup.TryLookup("hello", out value);

      // Assert
      Assert.IsTrue(result);
      Assert.AreEqual(value, "world!");
    }
  }
}
