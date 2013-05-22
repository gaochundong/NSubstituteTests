using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;
using NSubstitute.Experimental;

namespace NSubstitute.Tests
{
  [TestClass]
  public class CheckingCallOrder
  {
    public class Controller
    {
      private IConnection connection;
      private ICommand command;
      public Controller(IConnection connection, ICommand command)
      {
        this.connection = connection;
        this.command = command;
      }

      public void DoStuff() 
      {
        connection.Open();
        command.Run(connection);
        connection.Close();
      }
    }

    public class ICommand
    {
      public void Run(IConnection connection) { }
    }

    public class IConnection
    {
      public void Open() { }

      public void Close() { }

      public event Action SomethingHappened;
    }

    [TestMethod]
    public void Test_CheckingCallOrder_CommandRunWhileConnectionIsOpen()
    {
      var connection = Substitute.For<IConnection>();
      var command = Substitute.For<ICommand>();
      var subject = new Controller(connection, command);

      subject.DoStuff();

      Received.InOrder(() =>
      {
        connection.Open();
        command.Run(connection);
        connection.Close();
      });
    }

    [TestMethod]
    public void Test_CheckingCallOrder_SubscribeToEventBeforeOpeningConnection()
    {
      var connection = Substitute.For<IConnection>();
      connection.SomethingHappened += () => { /* some event handler */ };
      connection.Open();

      Received.InOrder(() =>
      {
        connection.SomethingHappened += Arg.Any<Action>();
        connection.Open();
      });
    }
  }
}
