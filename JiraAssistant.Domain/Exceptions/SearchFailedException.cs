using System;

namespace JiraAssistant.Domain.Exceptions
{
   public class SearchFailedException : Exception
   {
      public SearchFailedException(string message): base(message)
      {

      }
   }
}
