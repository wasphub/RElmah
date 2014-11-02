namespace RElmah.Models.Configuration
{
    public class Application
    {
        public static Application Create(string name)
        {
            return new Application(name);
        }

        Application(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
