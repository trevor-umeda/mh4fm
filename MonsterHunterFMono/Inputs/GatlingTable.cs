using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonsterHunterFMono
{
    public class GatlingTable
    {
        Dictionary<string, List<MoveInput>> gatlingTable = new Dictionary<string, List<MoveInput>>();

        public void addGatling(String moveName, List<MoveInput> gatling)
        {
            gatlingTable.Add(moveName, gatling);
        }

        public List<MoveInput> getPossibleGatlings(String moveName)
        {
            List<MoveInput> gatling = null;
            if (gatlingTable.TryGetValue(moveName, out gatling))
            {
                return gatling;
            }
            // Maybe this should return the default move list ( all moves are possible)
            //
            return null;
        }
    }
}
