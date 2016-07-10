using NLog;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JiraAssistant
{
    class Bootstrapper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

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
                        return assemblies[path];
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Failed to load: {0}", path);
                    }
                }

                return null;
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject == null)
                    return;

                _logger.Fatal(args.ExceptionObject as Exception, "Unexpected exception - shutting down.");
            };

            App.Main();
        }
    }
}
