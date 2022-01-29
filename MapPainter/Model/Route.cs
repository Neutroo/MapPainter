using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapPainter.Model
{
    public struct RoutePair
    {
        public double angle;
        public double path;
    }

    public class Route
    {
        private List<RoutePair> routePairs = new();
    }
}