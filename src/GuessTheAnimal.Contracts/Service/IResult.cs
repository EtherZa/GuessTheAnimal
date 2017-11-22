namespace GuessTheAnimal.Contracts.Service
{
    public interface IResult
    {
        string Comment { get; }

        Status Status { get; }

        string Token { get; }
    }
}