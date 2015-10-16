using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using LightShell.Messaging;
using LightShell.Messaging.Api;

namespace Yakuza.JiraClient.UnitTests.Messaging
{
   [TestFixture]
   public class MessageBusTests
   {
      private MessageBus _testedObject;

      public class MessageA : IMessage { }
      public class MessageB : IMessage { }

      [SetUp]
      public void TestSetup()
      {
         _testedObject = new MessageBus();
      }

      [Test]
      public void Registered_message_callback_is_called_once_message_is_sent()
      {
         bool wasCalled = false;
         _testedObject.Listen<MessageA>(m => wasCalled = true);

         _testedObject.Send(new MessageA());

         wasCalled.Should().BeTrue();
      }

      [Test]
      public void Registered_handler_is_called_once_message_is_sent()
      {
         var handlerMock = Substitute.For<IHandleMessage<MessageA>>();
         _testedObject.Register(handlerMock);

         _testedObject.Send(new MessageA());
         _testedObject.Send(new MessageB());

         handlerMock.Received(1).Handle(Arg.Any<MessageA>());
      }

      class FakeMultiHandler : IHandleMessage<MessageA>, IHandleMessage<MessageB>
      {
         public int MessageAReceivedCount { get; set; }
         public int MessageBReceivedCount { get; set; }

         public void Handle(MessageB message)
         {
            MessageBReceivedCount++;
         }

         public void Handle(MessageA message)
         {
            MessageAReceivedCount++;
         }
      }

      [Test]
      public void If_handler_handles_multiple_message_types_then_it_is_called_once_for_every_message_sent()
      {
         var handlerMock = new FakeMultiHandler();
         _testedObject.Register(handlerMock);

         _testedObject.Send(new MessageA());
         _testedObject.Send(new MessageB());

         handlerMock.MessageAReceivedCount.Should().Be(1);
         handlerMock.MessageBReceivedCount.Should().Be(1);
      }

      [Test]
      public void If_handler_handles_all_messages_it_should_be_called_on_every_sent_message()
      {
         var allHandler = Substitute.For<IHandleAllMessages>();
         _testedObject.SubscribeAllMessages(allHandler);

         _testedObject.Send(new MessageA());
         _testedObject.Send(new MessageB());

         allHandler.Received(2).Handle(Arg.Any<IMessage>());
      }
   }
}
