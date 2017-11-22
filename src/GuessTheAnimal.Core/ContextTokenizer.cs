namespace GuessTheAnimal.Core
{
    using System.Collections.Generic;
    using System.Linq;

    using GuessTheAnimal.Contracts.Service;

    using Newtonsoft.Json;

    public class ContextTokenizer : IContextTokenizer
    {
        private readonly IAnimalService animalService;

        public ContextTokenizer(IAnimalService animalService)
        {
            this.animalService = animalService;
        }

        public IContext GetContext(string token)
        {
            // TODO: decrypt
            var contextToken = JsonConvert.DeserializeObject<Token>(token);
            return this.ToContext(contextToken);
        }

        public string GetToken(IContext context)
        {
            // TODO: encrypt
            var token = this.ToToken(context);
            return JsonConvert.SerializeObject(token);
        }

        private IContext ToContext(Token token)
        {
            IEnumerable<IAttribute> GetAttributesByIdArray(int[] x)
            {
                if (x == null)
                {
                    return new IAttribute[0];
                }

                return from n in x
                       select this.animalService.GetAttribute(n);
            }

            return new Context
                       {
                           Current =
                               token.Current.HasValue
                                   ? this.animalService.GetAttribute(token.Current.Value)
                                   : null,
                           Included = GetAttributesByIdArray(token.Included),
                           Excluded = GetAttributesByIdArray(token.Excluded)
                       };
        }

        private Token ToToken(IContext context)
        {
            return new Token
                       {
                           Included = context.Included?.Select(x => x.Id)
                                             .ToArray(),
                           Excluded = context.Excluded?.Select(x => x.Id)
                                             .ToArray(),
                           Current = context.Current?.Id
                       };
        }

        private sealed class Context : IContext
        {
            public Context()
            {
                this.Included = new List<IAttribute>();
                this.Excluded = new List<IAttribute>();
            }

            public IAttribute Current { get; set; }

            public IEnumerable<IAttribute> Excluded { get; set; }

            public IEnumerable<IAttribute> Included { get; set; }
        }

        private sealed class Token
        {
            public int? Current { get; set; }

            public int[] Excluded { get; set; }

            public int[] Included { get; set; }
        }
    }
}