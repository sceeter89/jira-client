using System;

namespace JiraAssistant.Model.Exceptions
{
   public class SearchFailedException : Exception
   {
      public SearchFailedException(string message): base(message)
      {

      }
   }
}
