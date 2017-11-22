namespace GuessTheAnimal.Contracts.Repository
{
    using System.Collections.Generic;

    public interface IAnimalRepository
    {
        IEnumerable<IRepositoryAnimal> GetAll();
    }
}