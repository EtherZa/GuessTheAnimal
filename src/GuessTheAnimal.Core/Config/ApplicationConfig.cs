namespace GuessTheAnimal.Core.Config
{
    using GuessTheAnimal.Contracts.Config;

    public class ApplicationConfig : IApplicationConfig
    {
        public string ConnectionString { get; set; }
    }
}