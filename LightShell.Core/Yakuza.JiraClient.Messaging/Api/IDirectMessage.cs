namespace LightShell.Messaging.Api
{
   public interface IDirectMessage<TReplyType> : IMessage
                                    where TReplyType : IMessage
   {
      IHandleMessage<TReplyType> ReplyTo { get; }
   }
}
