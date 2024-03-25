using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace RPGInitiativeHelper
{
    internal class Combat
    {
        public List<Fighter> Combatants { get; set; }
        public int Turn { get; set; }
        public int InitiavePhase { get; set; }
        public Combat()
        {
            Combatants = new List<Fighter>();
            Turn = 1;
        }

        public Combat(List<Fighter> combatants)
        {
            Combatants = combatants;
            Turn = 1;
        }

        public int getStartInitiavePhase()
        {
            int retval = 0;

            if (Combatants.Count > 0)
            {
                retval = Combatants.Max(fighter => fighter.Initiative); ;
            }

            return retval;
        }

        public void nextInitiavePhase()
        {
            int temp = 0;

            foreach (Fighter f in Combatants)
                if (f.Initiative < InitiavePhase && f.Initiative > temp)
                    temp = f.Initiative;

            if (temp > 0)
                InitiavePhase = temp;
            else
            {
                Turn++;
                InitiavePhase = getStartInitiavePhase();
            }
        }
    }
}
