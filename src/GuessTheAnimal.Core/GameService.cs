namespace GuessTheAnimal.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using GuessTheAnimal.Contracts.Service;

    public class GameService : IGameService
    {
        private readonly IAnimalService animalService;

        private readonly IContextTokenizer contextTokenizer;

        public GameService(IAnimalService animalService, IContextTokenizer contextTokenizer)
        {
            this.animalService = animalService;
            this.contextTokenizer = contextTokenizer;
        }

        public IResult GetInitialQuestion()
        {
            this.GetAvailableItems(
                new IAttribute[0],
                new IAttribute[0],
                out var availableAnimals,
                out var availableAttributes);

            var context = new Context { Current = this.ChooseBestAttribute(availableAnimals, availableAttributes) };

            return new Result
                       {
                           Status = Status.Asking,
                           Comment = context.Current.Description,
                           Token = this.contextTokenizer.GetToken(context)
                       };
        }

        public IResult ProcessResult(string token, bool include)
        {
            var context = this.contextTokenizer.GetContext(token);
            var included = (include ? context.Included.Union(new[] { context.Current }) : context.Included).ToArray();

            var excluded = (include ? context.Excluded : context.Excluded.Union(new[] { context.Current })).ToArray();

            this.GetAvailableItems(included, excluded, out var availableAnimals, out var availableAttributes);

            if (availableAnimals.Length == 1)
            {
                return new Result
                           {
                               Status = Status.Success,
                               Comment = availableAnimals.Single()
                                   .Name
                           };
            }

            if (availableAttributes.Length == 0)
            {
                return new Result
                           {
                               Token = this.contextTokenizer.GetToken(context),
                               Status = Status.Error,
                               Comment =
                                   "Unable to determine animal as there is not enough variety in the attributes."
                           };
            }

            var question = this.ChooseBestAttribute(availableAnimals, availableAttributes);
            context = new Context { Included = included, Excluded = excluded, Current = question };

            return new Result
                       {
                           Token = this.contextTokenizer.GetToken(context),
                           Status = Status.Asking,
                           Comment = question.Description
                       };
        }

        internal void GetAvailableItems(
            IEnumerable<IAttribute> include,
            IEnumerable<IAttribute> exclude,
            out IAnimal[] availableAnimals,
            out IAttribute[] availableAttributes)
        {
            // rules:
            // exclude any animal that has an excluded attribute
            // exclude any animal that does not have all included attributes

            var excludeIds = exclude.Select(x => x.Id)
                .ToArray();
            var includeIds = include.Select(a => a.Id)
                .ToArray();

            var animals = (from a in this.animalService.GetAnimals()
                           where !a.Attributes.Select(x => x.Id)
                                     .Intersect(excludeIds)
                                     .Any()
                           select a).ToList();

            if (includeIds.Any())
            {
                animals.RemoveAll(
                    x => x.Attributes.Select(a => a.Id)
                             .Intersect(includeIds)
                             .Count() != includeIds.Length);
            }

            availableAnimals = animals.ToArray();

            availableAttributes = (from a in animals
                                   from t in a.Attributes
                                   group t by t.Id
                                   into g
                                   where !excludeIds.Contains(g.Key) && !includeIds.Contains(g.Key)
                                   select g.First()).ToArray();
        }

        private IAttribute ChooseBestAttribute(
            IEnumerable<IAnimal> availableAnimals,
            IEnumerable<IAttribute> availableAttributes)
        {
            var availableIds = availableAttributes.Select(x => x.Id)
                .ToArray();

            // Choose the question which is going to result in the greatest split
            var midpoint = (double)availableAnimals.Count() / 2;
            var q = from n in this.animalService.GetAttributes()
                    where availableIds.Contains(n.Key.Id)
                    select new { Attribute = n.Key, Deviation = Math.Abs(n.Value.Count() - midpoint) };

            return q.OrderBy(x => x.Deviation)
                .First()
                .Attribute;
        }

        private sealed class Context : IContext
        {
            public IAttribute Current { get; set; }

            public IEnumerable<IAttribute> Excluded { get; set; }

            public IEnumerable<IAttribute> Included { get; set; }
        }

        private sealed class Result : IResult
        {
            public string Comment { get; set; }

            public Status Status { get; set; }

            public string Token { get; set; }
        }
    }
}