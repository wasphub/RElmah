namespace RElmah.Models.Settings
{
    public class Settings
    {
        public Settings()
        {
            Bootstrap    = new BootstrapperSettings();
            Errors       = new ErrorsSettings();
            Domain       = new DomainSettings();
        }
        public BootstrapperSettings Bootstrap { get; set; }
        public ErrorsSettings Errors { get; set; }
        public DomainSettings Domain { get; set; }
    }
}