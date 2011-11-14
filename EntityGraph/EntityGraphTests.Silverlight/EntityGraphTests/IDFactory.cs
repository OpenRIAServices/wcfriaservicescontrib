namespace EntityGraphTests
{
    public static class IDFactory
    {
        private static int Id { get; set; }
        public static int Assign
        {
            get
            {
                if(AutoGenerateKeys == true)
                {
                    Id--; 
                    return Id;
                }
                else
                {
                    return 0;
                }
            }
        }
        public static bool AutoGenerateKeys { get; set; }
    }
}
