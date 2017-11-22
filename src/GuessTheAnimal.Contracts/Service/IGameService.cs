namespace GuessTheAnimal.Contracts.Service
{
    public interface IGameService
    {
        IResult GetInitialQuestion();

        IResult ProcessResult(string token, bool include);
    }
}