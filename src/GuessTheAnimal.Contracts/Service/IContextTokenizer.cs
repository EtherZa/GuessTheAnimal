namespace GuessTheAnimal.Contracts.Service
{
    public interface IContextTokenizer
    {
        IContext GetContext(string token);

        string GetToken(IContext context);
    }
}