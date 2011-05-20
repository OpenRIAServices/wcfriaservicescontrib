namespace EntityGraphTest
{
    public static class IDFactory
    {
        private static int Id { get; set; }
        public static int Assign { get { Id--; return Id; } }
    }
}
