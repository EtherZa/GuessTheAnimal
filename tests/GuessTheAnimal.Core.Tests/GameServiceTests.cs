namespace GuessTheAnimal.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using GuessTheAnimal.Contracts.Service;

    using Moq;

    using Xunit;

    public static class GameServiceTests
    {
        public class GetAvailableItems
        {
            [Fact]
            public void ExcludeAnimalsWhereAttributeHasBeenExplicitlyExcluded()
            {
                var includeAttributes = new IAttribute[0];
                var excludeAttributes = new IAttribute[] { new Attribute(1), new Attribute(2) };

                var includeAnimals = new[] { new Animal(3, 4), new Animal(3, 5) };
                var excludeAnimals = new[] { new Animal(1), new Animal(2, 3) };

                PerformInclusionTest(includeAnimals, excludeAnimals, includeAttributes, excludeAttributes);
            }

            [Fact]
            public void ExcludeAnimalsWhichDoNotHaveExplicitlyIncludedAttribute()
            {
                var includeAttributes = new IAttribute[] { new Attribute(3), new Attribute(4) };
                var excludeAttributes = new IAttribute[0];

                var includeAnimals = new[] { new Animal(3, 4), new Animal(3, 4, 5) };
                var excludeAnimals = new[] { new Animal(1), new Animal(2), new Animal(3) };

                PerformInclusionTest(includeAnimals, excludeAnimals, includeAttributes, excludeAttributes);
            }

            private static void PerformInclusionTest(
                Animal[] includeAnimals,
                Animal[] excludeAnimals,
                IAttribute[] includeAttributes,
                IAttribute[] excludeAttributes)
            {
                var mockAnimalService = new Mock<IAnimalService>(MockBehavior.Strict);
                mockAnimalService.Setup(x => x.GetAnimals())
                    .Returns(() => includeAnimals.Union(excludeAnimals))
                    .Verifiable();

                var mockContextTokenizer = new Mock<IContextTokenizer>(MockBehavior.Strict);

                IAttribute[] actualAttributes;
                var target = new GameService(mockAnimalService.Object, mockContextTokenizer.Object);
                target.GetAvailableItems(
                    includeAttributes,
                    excludeAttributes,
                    out var actualAnimals,
                    out actualAttributes);

                actualAnimals.ShouldAllBeEquivalentTo(includeAnimals);

                mockAnimalService.Verify(x => x.GetAnimals(), Times.Once);
            }

            private sealed class Animal : IAnimal
            {
                public Animal(params int[] attributeIds)
                {
                    this.Name = $"{Guid.NewGuid()}";

                    this.Attributes = (from a in attributeIds
                                       select new Attribute(a)).ToArray();
                }

                public IEnumerable<IAttribute> Attributes { get; }

                public string Name { get; }
            }

            private sealed class Attribute : IAttribute
            {
                public Attribute(int id)
                {
                    this.Id = id;
                }

                public string Description => $"Attribute {this.Id}";

                public int Id { get; }
            }
        }
    }
}