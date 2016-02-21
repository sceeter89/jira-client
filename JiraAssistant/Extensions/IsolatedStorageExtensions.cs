using System.IO;
using System.IO.IsolatedStorage;

namespace JiraAssistant.Extensions
{
   internal static class IsolatedStorageExtensions
   {
      public static void DeleteFolder(this IsolatedStorageFile iso, string path)
      {
         if (!iso.DirectoryExists(path))
            return;

         var folders = iso.GetDirectoryNames(Path.Combine(path, "*.*"));

         foreach (var folder in folders)
         {
            var folderName = Path.Combine(path, folder);
            iso.DeleteFolder(folderName);
         }

         foreach (var file in iso.GetFileNames(Path.Combine(path, "*.*")))
         {
            iso.DeleteFile(Path.Combine(path, file));
         }

         iso.DeleteDirectory(path);
      }
   }
}
