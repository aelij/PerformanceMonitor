using System;
using System.Collections.Generic;
using System.Reflection;

namespace PerformanceMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            LoadAssemblies();
        }

        #region Assembly Embedding

        private static readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        private static void LoadAssemblies()
        {
            var appAssembly = typeof (App).Assembly;
            foreach (var resourceName in appAssembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                {
                    using (var stream = appAssembly.GetManifestResourceStream(resourceName))
                    {
                        // ReSharper disable PossibleNullReferenceException
                        var assemblyData = new byte[(int) stream.Length];
                        // ReSharper restore PossibleNullReferenceException
                        stream.Read(assemblyData, 0, assemblyData.Length);

                        var assembly = Assembly.Load(assemblyData);
                        _assemblies.Add(assembly.GetName().Name, assembly);
                    }
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var shortName = new AssemblyName(args.Name).Name;
            Assembly assembly;
            if (_assemblies.TryGetValue(shortName, out assembly))
            {
                return assembly;
            }
            return null;
        }

        #endregion

    }
}
