namespace AsteroidsApp
{
    public class StatsSingleton
    {
        private static StatsSingleton _instance;
        private StatsSingleton()
        {
            Points = 0;
            Level = 0;
        }

        public static StatsSingleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new StatsSingleton();
                return _instance;
            }
        }

        public int Points { get; private set; }
        public int Level { get; private set; }

        public void AddPoints(int points)
        {
            Points += points;
        }

        public void LevelUp()
        {
            Level++;
        }

        public void Reset()
        {
            Points = 0;
            Level = 0;
        }
    }
}
