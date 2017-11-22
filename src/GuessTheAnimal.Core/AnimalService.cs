namespace GuessTheAnimal.Core
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using GuessTheAnimal.Contracts.Repository;
    using GuessTheAnimal.Contracts.Service;

    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository animalRepository;

        private readonly object syncRoot;

        private IAnimal[] animals;

        private IDictionary<IAttribute, IEnumerable<IAnimal>> attributes;

        private bool loaded;

        public AnimalService(IAnimalRepository animalRepository)
        {
            this.animalRepository = animalRepository;

            this.loaded = false;
            this.syncRoot = new object();
        }

        public IEnumerable<string> GetAnimalNames()
        {
            this.EnsureLoaded();

            return this.animals.Select(x => x.Name)
                .OrderBy(x => x);
        }

        public IEnumerable<IAnimal> GetAnimals()
        {
            this.EnsureLoaded();

            return this.animals;
        }

        public IAttribute GetAttribute(int id)
        {
            this.EnsureLoaded();

            return this.attributes.Keys.FirstOrDefault(x => x.Id == id);
        }

        public IDictionary<IAttribute, IEnumerable<IAnimal>> GetAttributes()
        {
            this.EnsureLoaded();

            return this.attributes;
        }

        internal void EnsureLoaded()
        {
            if (this.loaded)
            {
                return;
            }

            lock (this.syncRoot)
            {
                if (this.loaded)
                {
                    return;
                }

                this.animals = this.animalRepository.GetAll()
                    .Select(
                        x =>
                            {
                                return new Animal
                                           {
                                               Name = x.Name,
                                               Attributes = x.Attributes.Select(
                                                   y => new Attribute { Id = y.Id, Description = y.Description })
                                           };
                            })
                    .ToArray();

                // reverse the dictionary
                var attribs = new ConcurrentDictionary<int, IAttribute>();
                var lookup = new ConcurrentDictionary<IAttribute, List<IAnimal>>();
                foreach (var animal in this.animals)
                {
                    foreach (var attribute in animal.Attributes)
                    {
                        var key = attribs.GetOrAdd(attribute.Id, x => attribute);

                        lookup.GetOrAdd(key, k => new List<IAnimal>())
                            .Add(animal);
                    }
                }

                this.attributes = lookup.ToDictionary(k => k.Key, k => k.Value.Select(x => x));

                this.loaded = true;
            }
        }

        private sealed class Animal : IAnimal
        {
            public IEnumerable<IAttribute> Attributes { get; set; }

            public string Name { get; set; }
        }

        private sealed class Attribute : IAttribute
        {
            public string Description { get; set; }

            public int Id { get; set; }
        }
    }
}