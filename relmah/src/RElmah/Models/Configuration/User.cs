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
    }
}
