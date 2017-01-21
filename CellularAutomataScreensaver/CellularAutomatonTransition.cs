using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomataScreensaver
{
    class CellularAutomatonTransition
    {
        public bool[] Predecessors { get; private set; }
        public bool Successor { get; private set; }

        public CellularAutomatonTransition(bool[] predecessor, bool sucessor)
        {
            this.Predecessors = predecessor;
            this.Successor = sucessor;
        }

        internal bool DoeasPredecessorsMatch(bool[] predecessors)
        {
            if (predecessors.Length != this.Predecessors.Length) return false;
            return Enumerable.Range(0, this.Predecessors.Length)
                .All(x => predecessors[x] == this.Predecessors[x]);
        }
    }
}
