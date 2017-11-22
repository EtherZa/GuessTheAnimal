namespace GuessTheAnimal.Contracts.Service
{
    using System.Collections.Generic;

    public interface IAnimal
    {
        IEnumerable<IAttribute> Attributes { get; }

        string Name { get; }
    }
}