using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JiraAssistant
{
    class Bootstrapper
    {
        [STAThread]
        public static void Main()
        {
            var assemblies = new Dictionary<string, Assembly>();
            var executingAssembly = Assembly.GetExecutingAssembly();

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var assemblyName = new AssemblyName(e.Name);
                var path = string.Format("{0}.dll", assemblyName.Name);

                if (assemblies.ContainsKey(path))
                {
                    return assemblies[path];
                }

                using (var stream = executingAssembly.GetManifestResourceStream(path))
                {
                    if (stream == null)
                        return null;

                    var bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    try
                    {
                        assemblies.Add(path, Assembly.Load(bytes));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(string.Format("Failed to load: {0}, Exception: {1}", path, ex.Message));
                    }
                }

                return null;
            };

            App.Main();
        }
    }
}
