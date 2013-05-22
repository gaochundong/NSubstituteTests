using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class CreatingSubstitute
  {
    public interface ISomeInterface { }

    [TestMethod]
    public void Test_CreatingSubstitute_GetSubstitute()
    {
      var substitute = Substitute.For<ISomeInterface>();
    }

    public interface ICommand : IDisposable 
    {
      void Execute();
    }

    public class CommandRunner
    {
      private ICommand _command;

      public CommandRunner(ICommand command)
      {
        _command = command;
      }

      public void RunCommand()
      {
        _command.Execute();
        _command.Dispose();
      }
    }

    [TestMethod]
    public void Test_CreatingSubstitute_MultipleInterfaces()
    {
      var command = Substitute.For<ICommand, IDisposable>();

      var runner = new CommandRunner(command);
      runner.RunCommand();

      command.Received().Execute();
      ((IDisposable)command).Received().Dispose();
    }

    public class SomeClassWithCtorArgs : IDisposable
    {
      public SomeClassWithCtorArgs(int arg1, string arg2)
      {
      }

      public void Dispose() { }
    }

    [TestMethod]
    public void Test_CreatingSubstitute_SpecifiedOneClassType()
    {
      var substitute = Substitute.For(
            new[] { typeof(ICommand), typeof(IDisposable), typeof(SomeClassWithCtorArgs) },
            new object[] { 5, "hello world" }
          );
      Assert.IsInstanceOfType(substitute, typeof(ICommand));
      Assert.IsInstanceOfType(substitute, typeof(IDisposable));
      Assert.IsInstanceOfType(substitute, typeof(SomeClassWithCtorArgs));
    }

    [TestMethod]
    public void Test_CreatingSubstitute_ForDelegate()
    {
      var func = Substitute.For<Func<string>>();
      func().Returns("hello");
      Assert.AreEqual<string>("hello", func());
    }
  }
}
