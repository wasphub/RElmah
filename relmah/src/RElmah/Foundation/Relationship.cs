namespace RElmah.Foundation
{
    public static class Relationship
    {
        public static Relationship<TP, TS> Create<TP, TS>(TP primary, TS secondary)
        {
            return new Relationship<TP, TS>(primary, secondary);
        }
    }

    public class Relationship<TP, TS>
    {
        public Relationship(TP primary, TS secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }

        public TP Primary { get; private set; }
        public TS Secondary { get; private set; }
    }
}
