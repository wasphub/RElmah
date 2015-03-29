namespace RElmah.Client
{
    public class ClientToken
    {
        public ClientToken(string token)
        {
            Token = token;
        }

        public string Token { get; private set; }
    }
}