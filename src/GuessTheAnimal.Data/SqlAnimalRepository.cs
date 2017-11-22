namespace GuessTheAnimal.Data
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Dapper;

    using GuessTheAnimal.Contracts.Config;
    using GuessTheAnimal.Contracts.Repository;

    public class SqlAnimalRepository : IAnimalRepository
    {
        private readonly string connectionString;

        public SqlAnimalRepository(IApplicationConfig config)
        {
            this.connectionString = config.ConnectionString;
        }

        public IEnumerable<IRepositoryAnimal> GetAll()
        {
            const string Sql = @"SELECT A.Name, T.Id, T.Description
                                FROM dbo.Animal A
	                                INNER JOIN dbo.AnimalAttribute AA ON A.Id = AA.AnimalId
	                                INNER JOIN dbo.Attribute T ON AA.AttributeId = T.Id ";

            using (var con = new SqlConnection(this.connectionString))
            {
                con.Open();

                var dictionary = new ConcurrentDictionary<string, Animal>();
                return con.Query<Animal, Attribute, Animal>(
                              Sql,
                              (p, c) =>
                                  {
                                      var item = dictionary.GetOrAdd(p.Name, x => p);
                                      item.Add(c);
                                      return item;
                                  },
                              splitOn: "Id",
                              commandType: CommandType.Text)
                          .Distinct();
            }
        }

        private sealed class Animal : IRepositoryAnimal
        {
            private readonly List<IRepositoryAttribute> attributes;

            public Animal()
            {
                this.attributes = new List<IRepositoryAttribute>();
            }

            public IEnumerable<IRepositoryAttribute> Attributes => this.attributes;

            public string Name { get; set; }

            public void Add(IRepositoryAttribute attribute)
            {
                this.attributes.Add(attribute);
            }
        }

        private sealed class Attribute : IRepositoryAttribute
        {
            public string Description { get; set; }

            public int Id { get; set; }
        }
    }
}