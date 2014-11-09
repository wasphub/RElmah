namespace RElmah.Models.Configuration
{
    public class User
    {
        public static User Create(string name)
        {
            return new User(name);
        }

        User(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public string Token { get; private set; }

        public User AddToken(string token)
        {
            var u = new User(Name) { Token = token };
            return u;
        }
    }
}
