using System;
using System.Linq;
using System.Web.Mvc;
using Itransition.SendingEmails.Sending;
using Newtonsoft.Json;
using Stackoverflow.Models.Json;
using Stackoverflow.Models.Questions;
using Stackoverflow.SqlContext.Concrete.Answers;
using Stackoverflow.SqlContext.Concrete.Comments;
using Stackoverflow.SqlContext.Concrete.Questions;

namespace Stackoverflow.Controllers
{
    public class QuestionController : Controller
    {
        private const int UserId = 1;

        private readonly SendEmail _sendEmail = new SendEmail();
        private readonly QuestionRepository _questionRepository = new QuestionRepository();
        private readonly CommentaryQuestionRepository _commentaryQuestionRepository = new CommentaryQuestionRepository();
        private readonly CommentaryAnswerRepository _commentaryAnswerRepository = new CommentaryAnswerRepository();
        private readonly AnswerRepository _answerRepository = new AnswerRepository();

        private const string TypeAnswer = "Answer";
        private const string TypeQuestion = "Question";

        public ActionResult Index (int id)
        {
            var model = _questionRepository.Entity
                .Where(item => item.Id == id)
                .Select(item => new QuestionModel
                                    {
                                        Id = item.Id,
                                        Usefulness = item.Usefulness,
                                        Title = item.Title,
                                        Body = item.Body,
                                        PublicationDate = item.PublicationDate,
                                        User = new UserModel { Id = item.User.Id, DisplayName = item.User.Nickname},
                                    }).FirstOrDefault();

            if (model == null) RedirectToAction("Index", "Problems");

            model.QuestionComments = _commentaryQuestionRepository.Entity
                .Where(comment => comment.QuestionId == model.Id)
                .OrderBy(comment => comment.PublicationDate)
                .Select(comment =>
                        new CommentaryModel
                            {
                                Id = comment.Id,
                                Body = comment.Body,
                                PublicationDate = comment.PublicationDate,
                                User = new UserModel {Id = comment.User.Id, DisplayName = comment.User.Nickname}
                            }).ToList();

            model.Answers = _answerRepository.Entity.Where(answer => answer.QuestionId == model.Id)
                .OrderByDescending(answer => answer.Usefulness)
                .Select(answer =>
                        new AnswerModel
                            {
                                Id = answer.Id,
                                Usefulness = answer.Usefulness,
                                BestAnswer = answer.BestAnswer,
                                Body = answer.Body,
                                PublicationDate = answer.PublicationDate,
                                User = new UserModel { Id = answer.User.Id, DisplayName = answer.User.Nickname },
                            }).ToList();

            foreach (var answer in model.Answers)
            {
                answer.Comments = _commentaryAnswerRepository.Entity
                    .Where(commentaryAnswer => commentaryAnswer.AnswerId == answer.Id)
                    .OrderBy(item => item.PublicationDate)
                    .Select(commentaryAnswer =>
                            new CommentaryModel
                            {
                                Id = commentaryAnswer.Id,
                                PublicationDate = commentaryAnswer.PublicationDate,
                                Body = commentaryAnswer.Body,
                                User = new UserModel { Id = commentaryAnswer.User.Id, DisplayName = commentaryAnswer.User.Nickname }
                            }).ToList();
            }
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(QuestionModel model)
        {
            if (model.Answer != null)
            {
                _answerRepository.Add(UserId, model.Id, 0, model.Answer, DateTime.Now);
                _sendEmail.SendEmailAnswerToTheQuestion(model.Id);
            }
            return RedirectToAction("Index", "Question", new {id = model.Id});
        }

        public ActionResult EditAnswer(int id)
        {
            var model = _answerRepository.Entity
                .Where(item => item.Id == id)
                .Select(item => new AnswerModel
                {
                    Id = item.Id,
                    BestAnswer = item.BestAnswer,
                    Body = item.Body,
                    PublicationDate = item.PublicationDate,
                    Usefulness = item.Usefulness,
                    User = new UserModel { Id = item.User.Id, DisplayName = item.User.Nickname }
                }).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditAnswer(AnswerModel model)
        {
            var questionId = (int)_answerRepository.Entity.Where(item => item.Id == model.Id).Select(item => item.QuestionId).FirstOrDefault();
            _answerRepository.Edit(model.Id, model.Body);
            return RedirectToAction("Index", new { id = questionId });
        }

        public ActionResult DeleteAnswer(int id)
        {
            var model = _answerRepository.Entity
                .Where(item => item.Id == id)
                .Select(item => new AnswerModel
                                    {
                                        Id = item.Id,
                                        BestAnswer = item.BestAnswer,
                                        Body = item.Body,
                                        PublicationDate = item.PublicationDate,
                                        Usefulness = item.Usefulness,
                                        User = new UserModel {Id = item.User.Id, DisplayName = item.User.Nickname}
                                    }).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteAnswer(AnswerModel model)
        {
            var questionId = (int)_answerRepository.Entity.Where(item => item.Id == model.Id).Select(item => item.QuestionId).FirstOrDefault();
            _answerRepository.Delete(model.Id);
            return RedirectToAction("Index", new { id = questionId });
        }


        [HttpPost]
        [ValidateInput(false)]
        public string AddComment(int id, string type, string body)
        {
            if (String.CompareOrdinal(type, TypeAnswer) == 0)
            {
                _commentaryAnswerRepository.Add(UserId, id, body, DateTime.Now);
            }
            else if (String.CompareOrdinal(type, TypeQuestion) == 0)
            {
                _commentaryQuestionRepository.Add(UserId, id, body, DateTime.Now);
            }
            else
            {
                Response.StatusCode = 406;
                Response.Write("Wrong type of input values");
                return null;
            }
            var comments = new AnswerCommentaryModel
                               {
                                   Id = id,
                                   Type = type,
                                   Comment = new CommentaryModel
                                                 {
                                                     Body = body,
                                                     PublicationDate = DateTime.Now,
                                                     User = new UserModel {Id = UserId, DisplayName = "User1"}
                                                 }
                               };
            return JsonConvert.SerializeObject(comments);
        }

        [HttpPost]
        public string VoteUp (int id, string type)
        {
            var useful = new UsefulnessModel();
            if (String.CompareOrdinal(type, TypeAnswer) == 0)
            {
                _answerRepository.UsefulnessUp(id);
                
                useful.AmountLike = _answerRepository.Entity
                    .Where(item => item.Id == id)
                    .Select(item => item.Usefulness)
                    .FirstOrDefault();
                return JsonConvert.SerializeObject(useful);
            }
            if (String.CompareOrdinal(type, TypeQuestion) == 0)
            {
                _questionRepository.UsefulnessUp(id);

                useful.AmountLike = _questionRepository.Entity
                    .Where(item => item.Id == id)
                    .Select(item => item.Usefulness)
                    .FirstOrDefault();
                return JsonConvert.SerializeObject(useful);
            }
            Response.StatusCode = 406;
            Response.Write("Wrong type of input values");
            return null;
        }

        [HttpPost]
        public string VoteDown(int id, string type)
        {
            var useful = new UsefulnessModel();
            if (String.CompareOrdinal(type, TypeAnswer) == 0)
            {
                _answerRepository.UsefulnessDown(id);

                useful.AmountLike = _answerRepository.Entity
                    .Where(item => item.Id == id)
                    .Select(item => item.Usefulness)
                    .FirstOrDefault();
                return JsonConvert.SerializeObject(useful);
            }
            if (String.CompareOrdinal(type, TypeQuestion) == 0)
            {
                _questionRepository.UsefulnessDown(id);

                useful.AmountLike = _questionRepository.Entity
                    .Where(item => item.Id == id)
                    .Select(item => item.Usefulness)
                    .FirstOrDefault();
                return JsonConvert.SerializeObject(useful);
            }
            Response.StatusCode = 406;
            Response.Write("Wrong type of input values");
            return null;
        }

        [HttpPost]
        public void Usefulness (int id)
        {
            var questionId = _answerRepository.Entity.Where(item => item.Id == id).Select(item => item.QuestionId).FirstOrDefault();
            int countBestAnswer = _answerRepository.Entity
                .Where(item => item.QuestionId == questionId)
                .Count(item => item.BestAnswer);
            if (countBestAnswer == 0)
            {
                _answerRepository.SetBestAnswer(id);
            }
            else
            {
                _answerRepository.UpdateBestAnswer(id);
            }
        }
    }
}