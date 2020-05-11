namespace EnhancedDisastersMod
{
    public static class Helper
    {
        public static string[] GetMonths()
        {
            return new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        }

        static bool flag = true;
        public static float FramesPerDay
        {
            get
            {
                return 1.0f / (float)SimulationManager.instance.m_timePerFrame.TotalDays;
            }
        }

        public static float FramesPerYear
        {
            get
            {
                return FramesPerDay * 365f;
            }
        }
    }
}
