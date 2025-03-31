using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATR.Host.Core
{
    public abstract class Gauge
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public int Instance { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public abstract void Render();

        public abstract void Update(double delta);

    }
}
