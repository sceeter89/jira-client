using System;

namespace JiraAssistant.Model.Exceptions
{
   public class LoginFailedException : Exception
   {
      public LoginFailedException(string reason) : base(reason)
      {

      }
   }
}
