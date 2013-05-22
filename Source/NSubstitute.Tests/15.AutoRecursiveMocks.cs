using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class AutoRecursiveMocks
  {
    public interface INumberParser
    {
      int[] Parse(string expression);
    }
    public interface INumberParserFactory
    {
      INumberParser Create(char delimiter);
    }

    [TestMethod]
    public void Test_AutoRecursiveMocks_ManuallyCreateSubstitutes()
    {
      var factory = Substitute.For<INumberParserFactory>();
      var parser = Substitute.For<INumberParser>();
      factory.Create(',').Returns(parser);
      parser.Parse("an expression").Returns(new int[] { 1, 2, 3 });

      var actual = factory.Create(',').Parse("an expression");
      CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, actual);
    }

    [TestMethod]
    public void Test_AutoRecursiveMocks_AutomaticallyCreateSubstitutes()
    {
      var factory = Substitute.For<INumberParserFactory>();
      factory.Create(',').Parse("an expression").Returns(new int[] { 1, 2, 3 });

      var actual = factory.Create(',').Parse("an expression");
      CollectionAssert.AreEqual(new int[] { 1, 2, 3 }, actual);
    }

    [TestMethod]
    public void Test_AutoRecursiveMocks_CallRecursivelySubbed()
    {
      var factory = Substitute.For<INumberParserFactory>();
      factory.Create(',').Parse("an expression").Returns(new int[] { 1, 2, 3 });

      var firstCall = factory.Create(',');
      var secondCall = factory.Create(',');
      var thirdCallWithDiffArg = factory.Create('x');

      Assert.AreSame(firstCall, secondCall);
      Assert.AreNotSame(firstCall, thirdCallWithDiffArg);
    }

    public interface IContext
    {
      IRequest CurrentRequest { get; }
    }
    public interface IRequest
    {
      IIdentity Identity { get; }
      IIdentity NewIdentity(string name);
    }
    public interface IIdentity
    {
      string Name { get; }
      string[] Roles();
    }

    [TestMethod]
    public void Test_AutoRecursiveMocks_SubstituteChains()
    {
      var context = Substitute.For<IContext>();
      context.CurrentRequest.Identity.Name.Returns("My pet fish Eric");
      Assert.AreEqual(
        "My pet fish Eric",
        context.CurrentRequest.Identity.Name);
    }

    [TestMethod]
    public void Test_AutoRecursiveMocks_AutoValues()
    {
      var identity = Substitute.For<IIdentity>();
      Assert.AreEqual(string.Empty, identity.Name);
      Assert.AreEqual(0, identity.Roles().Length);
    }
  }
}
