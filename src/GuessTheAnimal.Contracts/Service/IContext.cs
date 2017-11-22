namespace GuessTheAnimal.Contracts.Service
{
    using System.Collections.Generic;

    public interface IContext
    {
        IAttribute Current { get; }

        IEnumerable<IAttribute> Excluded { get; }

        IEnumerable<IAttribute> Included { get; }
    }
}