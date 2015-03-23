namespace RElmah.Queries.Backend.Nulls
{
    public class QueriesFactory : IBackendQueriesFactory
    {
        private QueriesFactory() { }

        public static IBackendQueriesFactory Instance = new QueriesFactory();

        public void Setup()
        {
        }
    }
}
