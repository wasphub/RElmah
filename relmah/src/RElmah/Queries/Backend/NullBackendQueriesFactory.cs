namespace RElmah.Queries.Backend
{
    public class NullBackendQueriesFactory : IBackendQueriesFactory
    {
        private NullBackendQueriesFactory() { }

        public static IBackendQueriesFactory Instance = new NullBackendQueriesFactory();

        public void Setup()
        {
        }
    }
}
