using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Manhattanville
{
    interface IObservingTransform
    {
        void observe(BuildingTransform b);
    }
}
