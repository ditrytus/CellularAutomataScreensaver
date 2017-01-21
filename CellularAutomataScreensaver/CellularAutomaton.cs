using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomataScreensaver
{
    class CellularAutomaton
    {
        private CellularAutomatonRule rule;
        private bool edgeValue;

        public CellularAutomaton(byte ruleNumber, bool edgeValue)
        {
            this.rule = new CellularAutomatonRule(ruleNumber);
            this.edgeValue = edgeValue;
        }

        public bool[] Transform(bool[] generationVector)
        {
            bool[] nextGeneration = new bool[generationVector.Length];
            for (int i = 0; i < generationVector.Length; i++)
            {
                bool[] predecessors;
                if (i == 0)
                {
                    predecessors = new bool[] { edgeValue }.Concat(generationVector.Take(2)).ToArray();
                }
                else if (i == generationVector.Length - 1)
                {
                    predecessors = generationVector.Skip(generationVector.Length - 2).Concat(new bool[] { edgeValue }).ToArray();
                }
                else
                {
                    predecessors = generationVector.Skip(i - 1).Take(3).ToArray();
                }
                nextGeneration[i] = this.rule.Transform(predecessors);
            }
            return nextGeneration;
        }
    }
}
