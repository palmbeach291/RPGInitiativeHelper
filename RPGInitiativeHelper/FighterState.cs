namespace RPGInitiativeHelper
{
    public class FighterState
    {
        string name { get; set; }
        int rounds { get; set; }
        string description { get; set; }

        public FighterState(string name, int rounds, string description = "")
        {
            this.name = name;
            this.rounds = rounds;
            this.description = description;
        }

        public int DecreaseRounds()
        {
            rounds--;
            return rounds;
        }
    }
}
