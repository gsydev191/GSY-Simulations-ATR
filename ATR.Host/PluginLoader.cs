using ATR.Host.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ATR.Host
{
    public class PluginMetadata
    {
        public required string Name { get; set; }
        public required string Version { get; set; }
        public required Type PluginType { get; set; }
    }

    internal static class PluginLoader
    {
        public static List<PluginBase> LoadPlugins(string pluginFolder)
        {
            var plugins = new List<PluginBase>();

            if (!Directory.Exists(pluginFolder))
            {
                Console.WriteLine("Plugin folder not found!");
                return plugins;
            }

            var pluginFiles = Directory.GetFiles(pluginFolder, "*.dll");
            foreach (var file in pluginFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(PluginBase).IsAssignableFrom(t) && !t.IsAbstract);

                    foreach (var type in pluginTypes)
                    {
                        if (Activator.CreateInstance(type) is PluginBase pluginInstance)
                        {
                            pluginInstance.InitializePlugin();
                            plugins.Add(pluginInstance);
                            Console.WriteLine($"Loaded plugin: {pluginInstance.Name} v {pluginInstance.Version}");
                        }
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load plugin from {file}: {e.Message}");
                    throw;
                }
            }

            return plugins;
        }

        public static List<PluginMetadata> LoadPluginMetadata(string pluginFolder)
        {
            var pluginMetadataList = new List<PluginMetadata>();

            if (!Directory.Exists(pluginFolder))
            {
                Console.WriteLine("Plugin folder not found!");
                return pluginMetadataList;
            }

            var pluginFiles = Directory.GetFiles(pluginFolder, "*.dll");
            foreach (var file in pluginFiles)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(file);
                    var pluginTypes = assembly.GetTypes()
                        .Where(t => typeof(PluginBase).IsAssignableFrom(t) && !t.IsAbstract);

                    foreach (var type in pluginTypes)
                    {
                        var pluginInstance = Activator.CreateInstance(type) as PluginBase;
                        if(pluginInstance == null) continue;

                        pluginMetadataList.Add(new PluginMetadata
                        {
                            Name = pluginInstance.Name,
                            Version = pluginInstance.Version,
                            PluginType = type
                        });
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"Loader exception: {loaderException.Message}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load plugin from {file}: {e.Message}");
                    throw;
                }
            }

            return pluginMetadataList;
        }

        public static PluginBase? LoadPluginInstance(Type pluginType)
        {
            return Activator.CreateInstance(pluginType) as PluginBase;
        }
    }
}
