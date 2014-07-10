using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stackoverflow.Controllers;
using Stackoverflow.Models.Problems;
using Stackoverflow.Models.Questions;
using Stackoverflow.SqlContext.Concrete.Answers;
using Stackoverflow.SqlContext.Concrete.Comments;
using Stackoverflow.SqlContext.Concrete.Questions;

namespace Stackoverflow.UnitTest.ControllersTests
{
    [TestClass]
    public class ProblemsControllerTest
    {
        public ProblemsController Controller { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Controller = new ProblemsController();
        }

        [TestMethod]
        public void Index_SearchQuestionsOnAVisitedPage_ReturnQuestionAndVisitedPage()
        {
            // Arrange 
            const int page = 1;
            const int countNewsOnPage = 10;

            var questionRepository = new QuestionRepository();
            var countPages = (int)Math.Ceiling(questionRepository.Entity.Count() / (double)countNewsOnPage);

            int amountQuestion = questionRepository.Entity.Count();
            if (amountQuestion > countNewsOnPage)
            {
                amountQuestion = countNewsOnPage;
            }

            // act
            var actual = Controller.Index(page);

            //Assert
            Assert.IsNotNull(actual, "This test of Index controller ProblemController failed. Actual is null.");
            Assert.IsInstanceOfType(actual, typeof(ViewResult), "This test of Idex controller ProblemController failed. The return value does not match ViewResult.");

            var viewResult = (ViewResult)actual;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(ProblemsModel), "This test of Index controller ProblemController failed. The return value does not match ProblemsModel.");

            var resultModel = (ProblemsModel)viewResult.ViewData.Model;

            Assert.AreEqual(countPages, resultModel.CountPages, "This test of Index controller ProblemController failed. CounPage not identical.");
            Assert.AreEqual(amountQuestion, resultModel.Questions.Count, "This test of Index controller ProblemController failed. Amount Question is page not identical.");
            Assert.AreEqual(page, resultModel.VisitedPage, "This test of Index controller ProblemController failed. Visited page is not identical.");

        }

        [TestMethod]
        public void Add_ValidationAndPostQuestionModelInRepository_AddRecordToDb()
        {
            // Arrange 
            var model = new QuestionModel
            {
                Title = "Title",
                Body = "Body"
            };

            // Act
            var actual = Controller.Add(model) as RedirectToRouteResult;

            var questionRepository = new QuestionRepository();
            var newQuestionModel = questionRepository.Entity
                .Where(item => item.Title == model.Title && item.Body.Contains(model.Body))
                .Select(item => new QuestionModel
                {
                    Id = item.Id,
                    Usefulness = item.Usefulness,
                    Title = item.Title,
                    Body = item.Body,
                    PublicationDate = item.PublicationDate,
                    User = new UserModel { Id = item.User.Id, DisplayName = item.User.Nickname },
                }).FirstOrDefault();

            // Assert
            Assert.IsNotNull(actual, "The test of Add() controller ProblemsController failed. Actual is null");
            Assert.AreEqual("Index", actual.RouteValues["Action"], "The test of Add() controller ProblemsController failed. Not redirected to the - Index page");
            Assert.IsNotNull(newQuestionModel, "The test of Add() controller ProblemsController failed. Data do not match"); 

            // Delete
            questionRepository.Delete(newQuestionModel.Id);
        }

        [TestMethod]
        public void Edit_SearchRecordsAndEditPostQuestionModelInRepository_ChangeRecordsInDb()
        {
            // Arrange 
            const int userId = 1;
            var model = new QuestionModel
            {
                Title = "Title",
                Body = "Body",
                Usefulness = 0,
                PublicationDate = DateTime.Now
            };
            var questionRepository = new QuestionRepository();
            questionRepository.Add(userId, model.Title, model.Body, model.Usefulness, model.PublicationDate);
            
            model.Id = questionRepository.Entity
                                            .Where(item => item.Title == model.Title && item.Body.Contains(model.Body))
                                            .Select(item => item.Id)
                                            .SingleOrDefault();
            // Act
            ActionResult actual = Controller.Edit((int)model.Id);

            //Assert
            Assert.IsNotNull(actual, "The test of Edit() controller ProblemsController failed. actual is null");
            Assert.IsInstanceOfType(actual, typeof(ViewResult), "The test of Edit() controller ProblemsController failed. ViewResult not identical");

            var viewResult = (ViewResult)actual;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(QuestionModel), "The test of Edit() controller ProblemsController failed. ViewData not identical QuestionModel");

            var resultModel = (QuestionModel)viewResult.ViewData.Model;
            Assert.AreEqual(resultModel.Id, model.Id, "The test of Edit() controller ProblemsController failed. Id not identical");
            Assert.AreEqual(resultModel.PublicationDate.ToString(CultureInfo.InvariantCulture), model.PublicationDate.ToString(CultureInfo.InvariantCulture), 
                "The test of Edit() controller ProblemsController failed. PublicationDate not identical");
            Assert.AreEqual(resultModel.Title, model.Title, "The test of Edit() controller ProblemsController failed. Title not identical");
            Assert.AreEqual(resultModel.Body, model.Body, "The test of Edit() controller ProblemsController failed. Body not identical");
            Assert.AreEqual(resultModel.Usefulness, model.Usefulness, "The test of Edit() controller ProblemsController failed. Usefulness not identical");
            Assert.AreEqual(resultModel.User.Id, userId, "The test of Edit() controller ProblemsController failed. UserId not identical");

            // delete
            questionRepository.Delete(model.Id);
        }
    
        [TestMethod]
        public void Delete_SearchRecordsAndDeletePostQuestionSummaryModelInRepository_DeleteRecordsFromDb()
        {
            // Arrange 
            const int userId = 1;
            // add question
            var questionRepository = new QuestionRepository();
            questionRepository.Add(userId, "Title", "Body", 0, DateTime.Now);

            var questionId = questionRepository.Entity
                                            .Where(item => item.Title == "Title" && item.Body.Contains("Body"))
                                            .Select(item => item.Id)
                                            .SingleOrDefault();
            // add answer 
            var answerRepository = new AnswerRepository();
            answerRepository.Add(userId, questionId, 0, "Body Answer", DateTime.Now );
            var answerId = answerRepository.Entity
                                        .Where(item => item.QuestionId == questionId && item.Body.Contains("Body Answer"))
                                        .Select(item => item.Id)
                                        .SingleOrDefault();
            
            // add commentary answer
            var commentaryAnswerRepository = new CommentaryAnswerRepository();
            commentaryAnswerRepository.Add(userId, answerId, "Body Commentary Answer", DateTime.Now);
            var commentaryAnswerId = commentaryAnswerRepository.Entity
                                          .Where(item => item.AnswerId == answerId && item.Body.Contains("Body Commentary Answer"))
                                          .Select(item => item.Id)
                                          .SingleOrDefault();
            // add commentary question
            var commentaryQuestionRepository = new CommentaryQuestionRepository();
            commentaryQuestionRepository.Add(userId, questionId, "Body Commentary Question", DateTime.Now);
            var commentaryQuestionId = commentaryQuestionRepository.Entity
                                            .Where(item => item.QuestionId == questionId && item.Body.Contains("Body Commentary Question"))
                                            .Select(item => item.Id)
                                            .SingleOrDefault();

            var model = questionRepository.Entity
                .Where(question => question.Id == questionId)
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

            // Act
            var actual = Controller.Delete(model) as RedirectToRouteResult;

            //Assert
            Assert.IsNotNull(actual, "The test of Delete() controller ProblemsController failed. actual is null");
            Assert.AreEqual("Index", actual.RouteValues["Action"], "The test of Delete() controller ProblemsController failed. Not redirected to the - Index page");

            var deleteCommentaryQuestion = commentaryQuestionRepository.Entity.SingleOrDefault(item => item.Id == commentaryQuestionId);
            Assert.IsNull(deleteCommentaryQuestion, "The test of Delete() controller ProblemsController failed. Comment the question is not removed");

            var deleteCommentaryAnswer = commentaryAnswerRepository.Entity.SingleOrDefault(item => item.Id == commentaryAnswerId);
            Assert.IsNull(deleteCommentaryAnswer, "The test of Delete() controller ProblemsController failed. Comment the answer is not removed");

            var deleteAnswer = answerRepository.Entity.SingleOrDefault(item => item.Id == answerId);
            Assert.IsNull(deleteAnswer, "The test of Delete() controller ProblemsController failed. The answer is not removed");

            var deleteQuestion = questionRepository.Entity.SingleOrDefault(item => item.Id == questionId);
            Assert.IsNull(deleteQuestion, "The test of Delete() controller ProblemsController failed. The question is not removed");

        }
        
    }
}
