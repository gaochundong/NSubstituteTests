using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ArgumentMatchers
  {
    public interface ICalculator
    {
      int Add(int a, int b);
      string Mode { get; set; }
    }

    [TestMethod]
    public void Test_ArgumentMatchers_IgnoringArguments()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Add(Arg.Any<int>(), 5).Returns(7);

      Assert.AreEqual(7, calculator.Add(42, 5));
      Assert.AreEqual(7, calculator.Add(123, 5));
      Assert.AreNotEqual(7, calculator.Add(1, 7));
    }

    public interface IFormatter
    {
      string Format(object o);
    }

    [TestMethod]
    public void Test_ArgumentMatchers_MatchSubTypes()
    {
      IFormatter formatter = Substitute.For<IFormatter>();

      formatter.Format(new object());
      formatter.Format("some string");

      formatter.Received().Format(Arg.Any<object>());
      formatter.Received().Format(Arg.Any<string>());
      formatter.DidNotReceive().Format(Arg.Any<int>());
    }

    [TestMethod]
    public void Test_ArgumentMatchers_ConditionallyMatching()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Add(1, -10);

      // 检查接收到第一个参数为1，第二个参数小于0的调用
      calculator.Received().Add(1, Arg.Is<int>(x => x < 0));
      // 检查接收到第一个参数为1，第二个参数为 -2、-5和-10中的某个数的调用
      calculator
        .Received()
        .Add(1, Arg.Is<int>(x => new[] { -2, -5, -10 }.Contains(x)));
      // 检查未接收到第一个参数大于10，第二个参数为-10的调用
      calculator.DidNotReceive().Add(Arg.Is<int>(x => x > 10), -10);
    }

    [TestMethod]
    public void Test_ArgumentMatchers_ConditionallyMatchingThrowException()
    {
      IFormatter formatter = Substitute.For<IFormatter>();

      formatter.Format(Arg.Is<string>(x => x.Length <= 10)).Returns("matched");

      Assert.AreEqual("matched", formatter.Format("short"));
      Assert.AreNotEqual("matched", formatter.Format("not matched, too long"));

      // 此处将不会匹配，因为在尝试访问 null 的 Length 属性时会抛出异常，
      // 而 NSubstitute 会假设其为不匹配并隐藏掉异常。
      Assert.AreNotEqual("matched", formatter.Format(null));
    }

    [TestMethod]
    public void Test_ArgumentMatchers_MatchingSpecificArgument()
    {
      var calculator = Substitute.For<ICalculator>();

      calculator.Add(0, 42);

      // 这里可能不工作，NSubstitute 在这种情况下无法确定在哪个参数上应用匹配器
      //calculator.Received().Add(0, Arg.Any<int>());

      calculator.Received().Add(Arg.Is(0), Arg.Any<int>());
    }
  }
}
