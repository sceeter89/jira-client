using System;

namespace JiraAssistant.Domain.Exceptions
{
   public class LoginFailedException : Exception
   {
      public LoginFailedException(string reason) : base(reason)
      {

      }
   }
}
