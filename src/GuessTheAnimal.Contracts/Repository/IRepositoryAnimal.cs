namespace GuessTheAnimal.Contracts.Repository
{
    using System.Collections.Generic;

    public interface IRepositoryAnimal
    {
        IEnumerable<IRepositoryAttribute> Attributes { get; }

        string Name { get; }
    }
}