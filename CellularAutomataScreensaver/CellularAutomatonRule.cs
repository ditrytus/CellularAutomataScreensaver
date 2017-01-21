using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellularAutomataScreensaver
{
    class CellularAutomatonRule
    {
        private IEnumerable<CellularAutomatonTransition> transitions;

        public CellularAutomatonRule(byte number)
        {
            var numOfTransitions = (int)Math.Pow(2, 3);
            var binaryNumber = ConvertToBinary(number, numOfTransitions);

            List<CellularAutomatonTransition> transitionsList = new List<CellularAutomatonTransition>();

            for (int i = (int)(numOfTransitions - 1); i >= 0; i--)
            {
                var binaryTrasitionPredecessors = ConvertToBinary(i, 3);
                transitionsList.Add(new CellularAutomatonTransition(binaryTrasitionPredecessors, binaryNumber[i]));
            }

            this.transitions = transitionsList;
        }

        private static bool[] ConvertToBinary(int number, int width)
        {
            return Convert.ToString(number, 2).PadLeft(width, '0').Select(x => x == '0' ? false : true).ToArray();
        }

        public bool Transform(bool[] predecessors)
        {
            return transitions.Single(x => x.DoeasPredecessorsMatch(predecessors)).Successor;
        }
    }
}
