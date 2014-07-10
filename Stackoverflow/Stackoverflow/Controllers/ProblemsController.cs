using System;
using System.Linq;
using System.Web.Mvc;
using Stackoverflow.Models.Problems;
using Stackoverflow.Models.Questions;
using Stackoverflow.SqlContext.Concrete.Questions;

namespace Stackoverflow.Controllers
{
    public class ProblemsController : Controller
    {
        private readonly QuestionRepository _questionRepository = new QuestionRepository();
        private const int CountNewsOnPage = 10;
        private const int UserId = 1;

        public ActionResult Index(int page)
        {
            if (page < 1) page = 1;

            var questions = _questionRepository.Entity
                .OrderByDescending(item => item.PublicationDate)
                .Skip(CountNewsOnPage*(page - 1))
                .Take(CountNewsOnPage)
                .Select(item => new QuestionSummaryModel
                                    {
                                        Id = item.Id,
                                        Body = item.Body,
                                        PublicationDate = item.PublicationDate,
                                        Title = item.Title,
                                        Usefulness = item.Usefulness,
                                        User = new UserModel
                                                   {
                                                       Id = item.User.Id,
                                                       DisplayName = item.User.Nickname
                                                   },
                                        AmountAnswer = item.Answer.Count(),
                                        BestAnswer = item.Answer.Count(answer => answer.BestAnswer)
                                    }).ToList();

            var model = new ProblemsModel
                            {
                                Questions = questions,
                                CountPages =
                                    (int) Math.Ceiling(_questionRepository.Entity.Count()/(double) CountNewsOnPage),
                                VisitedPage = page
                            };
            return View(model);
        }

        public ActionResult Add ()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add (QuestionModel model)
        {
            if (!string.IsNullOrEmpty(model.Title) && !string.IsNullOrEmpty(model.Body))
            {
                _questionRepository.Add(UserId, model.Title, model.Body, 0, DateTime.Now);
                return RedirectToAction("Index");
            }
            return View(model); 
        }

        public ActionResult Edit(int id)
        {
            var model = _questionRepository.Entity
                .Where(question => question.Id == id)
                .Select(question =>
                        new QuestionModel
                        {
                            Id = question.Id,
                            Usefulness = question.Usefulness,
                            Title = question.Title,
                            Body = question.Body,
                            PublicationDate = question.PublicationDate,
                            User = new UserModel
                            {
                                Id = question.User.Id,
                                DisplayName = question.User.Nickname
                            }
                        }).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(QuestionModel model)
        {
            if (model.Title != null && model.Body != null)
            {
                _questionRepository.Edit(model.Id, model.Title, model.Body);
                return RedirectToAction("Index", "Question", new {id = model.Id});
            }
            return View(model);
        }
    
        public ActionResult Delete(int id)
        {
            var model = _questionRepository.Entity
                .Where(question => question.Id == id)
                .Select(item => new QuestionSummaryModel
                    {
                        Id = item.Id,
                        Body = item.Body,
                        PublicationDate = item.PublicationDate,
                        Title = item.Title,
                        Usefulness = item.Usefulness,
                        User = new UserModel
                        {
                            Id = item.User.Id,
                            DisplayName = item.User.Nickname
                        },
                        AmountAnswer = item.Answer.Count(),
                        BestAnswer = item.Answer.Count(answer => answer.BestAnswer)
                    }).FirstOrDefault();

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(QuestionSummaryModel model)
        {
            _questionRepository.Delete(model.Id);
            return RedirectToAction("Index");
        }

    }
}
