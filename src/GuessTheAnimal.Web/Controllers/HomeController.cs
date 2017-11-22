namespace GuessTheAnimal.Web.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using GuessTheAnimal.Contracts.Service;
    using GuessTheAnimal.Web.Models;

    using Microsoft.AspNetCore.Mvc;

    public partial class HomeController : Controller
    {
        private readonly IAnimalService animalService;

        private readonly IGameService gameService;

        public HomeController(IAnimalService animalService, IGameService gameService)
        {
            this.animalService = animalService;
            this.gameService = gameService;
        }

        [HttpGet]
        public virtual IActionResult Answer(AnswerViewModel model)
        {
            return this.View(model);
        }

        public virtual IActionResult Error()
        {
            return this.View(
                new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier
                    });
        }

        [HttpGet]
        public virtual IActionResult HandledError(HandledErrorViewModel model)
        {
            return this.View(model);
        }

        public virtual IActionResult Index()
        {
            var model = new IndexViewModel
                            {
                                Animals = this.animalService.GetAnimalNames()
                                              .OrderBy(x => x)
                                              .ToArray()
                            };

            return this.View(model);
        }

        public virtual IActionResult Quiz()
        {
            var q = this.gameService.GetInitialQuestion();
            var model = new QuizViewModel
                            {
                                Token = q.Token,
                                Question = q.Comment
                            };

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual IActionResult Quiz([FromForm] QuizViewModel model)
        {
            var result = this.gameService.ProcessResult(model.Token, model.Include);
            switch (result.Status)
            {
                case Status.Error:
                    return this.RedirectToAction(
                        nameof(this.HandledError),
                        new HandledErrorViewModel
                            {
                                Description = result.Comment
                            });

                case Status.Success:
                    return this.RedirectToAction(
                        nameof(this.Answer),
                        new AnswerViewModel
                            {
                                Name = result.Comment
                            });

                case Status.Asking:
                    var quizViewModel = new QuizViewModel
                                            {
                                                Token = result.Token,
                                                Question = result.Comment
                                            };
                    return this.View(quizViewModel);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}