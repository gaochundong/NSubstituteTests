using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;

namespace NSubstitute.Tests
{
  [TestClass]
  public class ClearReceivedCalls
  {
    public interface ICommand
    {
      void Execute();
    }

    public class OnceOffCommandRunner
    {
      ICommand command;
      public OnceOffCommandRunner(ICommand command)
      {
        this.command = command;
      }
      public void Run()
      {
        if (command == null) return;
        command.Execute();
        command = null;
      }
    }

    [TestMethod]
    public void Test_ClearReceivedCalls_ForgetPreviousCalls()
    {
      var command = Substitute.For<ICommand>();
      var runner = new OnceOffCommandRunner(command);

      // 第一次运行
      runner.Run();
      command.Received().Execute();

      // 忘记前面对command的调用
      command.ClearReceivedCalls();

      // 第二次运行
      runner.Run();
      command.DidNotReceive().Execute();
    }
  }
}
