using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

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

        public void addFighter(string name = "Kämpfer")
        {
            int count = 0;
            string newName = name;

            while (fighterContained(newName))
            {
                newName = name + "_" + count.ToString();
                count++;
            }

            Combatants.Add(new Fighter(newName,1,1));
        }

        public bool fighterContained(string name)
        {
            bool retval = false;

            foreach(Fighter f in Combatants)
                if(f.Name==name)
                    retval = true;

            return retval;
        }
    }
}
