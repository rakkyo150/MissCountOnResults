using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissCountOnResults
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        public bool Enable { get; set; }
    }
}
