using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATR.Host.Core
{
    public abstract class PluginBase
    {
        public List<Gauge> Gauges { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string Version { get; protected set; }

        protected PluginBase()
        {
            Gauges = new List<Gauge>();
            Name = "Default Plugin Name";
            Version = "1.0";
            Description = "Default Description";
        }

        public abstract void InitializePlugin();

        public List<Gauge> LoadGauges()
        {
            return Gauges;
        }
    }
}
