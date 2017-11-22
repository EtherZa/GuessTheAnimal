namespace GuessTheAnimal.Core.Tests
{
    using GuessTheAnimal.Contracts.Repository;

    using Moq;

    using Xunit;

    public static class AnimalServiceTests
    {
        public class EnsureLoadedTests
        {
            [Fact]
            public void RepositoryIsCalledOnlyOnce()
            {
                var mockRepo = new Mock<IAnimalRepository>(MockBehavior.Strict);
                mockRepo.Setup(x => x.GetAll())
                        .Returns(() => new IRepositoryAnimal[0])
                        .Verifiable();

                var target = new AnimalService(mockRepo.Object);

                // call twice
                target.EnsureLoaded();
                target.EnsureLoaded();

                mockRepo.Verify(x => x.GetAll(), Times.Once);
            }
        }
    }
}