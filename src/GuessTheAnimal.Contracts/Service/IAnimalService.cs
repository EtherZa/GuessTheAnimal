namespace GuessTheAnimal.Contracts.Service
{
    using System.Collections.Generic;

    public interface IAnimalService
    {
        IEnumerable<string> GetAnimalNames();

        IEnumerable<IAnimal> GetAnimals();

        IAttribute GetAttribute(int id);

        IDictionary<IAttribute, IEnumerable<IAnimal>> GetAttributes();
    }
}