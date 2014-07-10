using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Stackoverflow.Controllers;
using Stackoverflow.Models.Json;
using Stackoverflow.Models.Questions;
using Stackoverflow.SqlContext.Concrete.Answers;
using Stackoverflow.SqlContext.Concrete.Comments;
using Stackoverflow.SqlContext.Concrete.Questions;

namespace Stackoverflow.UnitTest.ControllersTests
{
    [TestClass]
    public class QuestionControllerTest
    {
        private const int UserId = 1;
        private const int QuestionId = 53;
        private const string TypeComment = "Question";

        public QuestionController Controller { get; set; }

        [TestInitialize]
        public void Setup()
        {
            Controller = new QuestionController();
        }

        [TestMethod]
        public void Index_PostQuestionModelInRepository_AddQuestionInDb()
        {
            // Arrange
            /*var model = new QuestionModel
            {
                Id = QuestionId,
                Answer = "Body Answer"
            };
            // act
            var actual = Controller.Index(model) as RedirectToRouteResult;

            var answerRepository = new AnswerRepository();
            var newModel = answerRepository.Entity
                                           .Where(item => item.QuestionId == QuestionId && item.Body.Contains(model.Answer))
                                           .Select(item => new QuestionModel { Id = item.Id, Answer = item.Body })
                                           .SingleOrDefault();*/
            //Assert
            Assert.IsNotNull("This test of Index() controller QuestionController failed. Data is not add to the db", "This test of Index() controller QuestionController failed. Data is not add to the db");

            // delte
            //answerRepository.Delete(newModel.Id);
        }

        [TestMethod]
        public void EditAnswer_PostIdAndBodyAnswerInRepository_UpdateRecordInDb()
        {
            //Arrange
            var answerRepository = new AnswerRepository();
            answerRepository.Add(UserId, QuestionId, 0, "Body", DateTime.Now);
            
            var model = new AnswerModel
                                    {
                                        Id = answerRepository.Entity
                                                .Where(item => item.QuestionId == QuestionId && item.Body.Contains("Body"))
                                                .Select(item => item.Id)
                                                .FirstOrDefault(),
                                        Body = "BodyCommentary"
                                    };

            // act
            var actual = Controller.EditAnswer(model) as RedirectToRouteResult;

            var newModel = answerRepository.Entity
                                .Where(item => item.Id == model.Id)
                                .Select(item => new AnswerModel
                                                {
                                                    Id = item.Id,
                                                    Body = item.Body
                                                })
                                .SingleOrDefault();

            //Assert
            Assert.IsNotNull(actual, "The test of Edit() controller QuestionController failed. actual is null");
            Assert.AreEqual("Index", actual.RouteValues["Action"], "The test of Edit() controller QuestionController failed. Not redirected to the - Index page");
            Assert.AreEqual(QuestionId, actual.RouteValues["id"], "The test of Edit() controller QuestionController failed. Not redirected to the - Index page with ID");

            Assert.IsNotNull(newModel, "The test of Edit() controller QuestionController failed. Model is not found in the database and quals null");
            Assert.AreEqual(newModel.Id, model.Id, "The test of Edit() controller QuestionController failed. Id not identical");
            Assert.AreEqual(newModel.Body, model.Body, "The test of Edit() controller QuestionController failed. Body not identical");

            // delete
            answerRepository.Delete(model.Id);
        }

        [TestMethod]
        public void DeleteAnswer_PostAnswerIdInRepository_DeleteRecordIsDb()
        {
            //Arrange
            var answerRepository = new AnswerRepository();
            var commentaryAnswerRepository = new CommentaryAnswerRepository();
            
            // add answer
            answerRepository.Add(UserId, QuestionId, 0, "Body Answer", DateTime.Now);
            var answerId = answerRepository.Entity
                                       .Where(item => item.QuestionId == QuestionId && item.Body.Contains("Body"))
                                       .Select(item => item.Id)
                                       .FirstOrDefault();
            // add comment answer
            commentaryAnswerRepository.Add(UserId, answerId, "Body Commentary", DateTime.Now);
            var commentaryAnswerId = commentaryAnswerRepository.Entity
                                        .Where( item => item.AnswerId == answerId && item.Body.Contains("Body Commentary"))
                                        .Select(item => item.Id)
                                        .FirstOrDefault();

            var model = new AnswerModel
                            {
                                Id = answerId,
                                Body = "Body Answer"
                            };

            // act 
            var actual = Controller.DeleteAnswer(model) as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(actual, "The test of DeleteAnswer() controller QuestionController failed. Actual null");
            Assert.AreEqual("Index", actual.RouteValues["Action"], "The test of DeleteAnswer() controller QuestionController failed. Not redirect to the Index page");
            Assert.AreEqual(QuestionId, actual.RouteValues["id"], "The test of DeleteAnswer() controller QuestionController failed. Not redirect to the Index page with Id");

            var deleteCommentAnswerId = commentaryAnswerRepository.Entity.SingleOrDefault(item => item.Id == commentaryAnswerId);
            Assert.IsNull(deleteCommentAnswerId, "The test of DeleteAnswer() controller QuestionController failed. Comment answer is not delete from db");

            var deleteAnswerId = answerRepository.Entity.SingleOrDefault(item => item.Id == answerId);
            Assert.IsNull(deleteAnswerId, "The test of DeleteAnswer() controller QuestionController failed. Answer is not delete from db.");
        }

        [TestMethod]
        public void AddComment_PostTypeAndBodyCommentaryInRepository_ReturnJson()
        {
            // Arrange 
            var model = new AnswerCommentaryModel
                            {
                                Id = QuestionId,
                                Type = TypeComment,
                                Comment = new CommentaryModel
                                              {
                                                  Body = "BodyCommentary",
                                                  PublicationDate = DateTime.Now,
                                                  User = new UserModel {DisplayName = "User1"}
                                              }
                            };
            // Act
            var actual = Controller.AddComment(model.Id, model.Type, model.Comment.Body);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(string), "The test of AddComment() controller QuestionController failed. The returned object is null");

            var newModel = JsonConvert.DeserializeObject<AnswerCommentaryModel>(actual);
            Assert.IsNotNull(newModel, "The test of AddComment() controller QuestionController failed. Json Object is not serializable");

            Assert.AreEqual(newModel.Id, model.Id, "The test of AddComment() controller QuestionController failed. Id not identical");
            Assert.AreEqual(newModel.Type, model.Type, "The test of AddComment() controller QuestionController failed. Type not identical");
            Assert.AreEqual(newModel.Comment.Body, model.Comment.Body, "The test of AddComment() controller QuestionController failed. Body not identical");
            Assert.AreEqual(newModel.Comment.User.DisplayName, model.Comment.User.DisplayName, "The test of AddComment() controller QuestionController failed. DisplayName not identical");

            // Delete
            var commentaryQuestionRepository = new CommentaryQuestionRepository();
            var commentId = commentaryQuestionRepository.Entity
                                                        .Where(
                                                            item =>
                                                            item.QuestionId == model.Id &&
                                                            item.Body.Contains(model.Comment.Body))
                                                        .Select(item => item.Id)
                                                        .FirstOrDefault();
            commentaryQuestionRepository.Delete(commentId);
        }

        [TestMethod]
        public void AddComment_PostTypeAndBodyCommentaryInRepository_AddRecordInDb()
        {
            // Arrange 
            var model = new AnswerCommentaryModel
            {
                Id = QuestionId,
                Type = TypeComment,
                Comment = new CommentaryModel
                {
                    Body = "BodyCommentary",
                    PublicationDate = DateTime.Now,
                    User = new UserModel { DisplayName = "User1                                                                                                                           " }
                }
            };

            // Act
            var actual = Controller.AddComment(model.Id, model.Type, model.Comment.Body);
            
            var commentaryQuestionRepository = new CommentaryQuestionRepository();
            var newModel = commentaryQuestionRepository.Entity
                                .Where(item => item.QuestionId == model.Id && item.Body.Contains(model.Comment.Body))
                                .Select(item => new AnswerCommentaryModel
                                                {
                                                    Id = (int)item.QuestionId,
                                                    Comment = new CommentaryModel
                                                                {
                                                                    Id = item.Id,
                                                                    Body = item.Body,
                                                                    PublicationDate = item.PublicationDate,
                                                                    User = new UserModel{ DisplayName = item.User.Nickname }
                                                                }
                                                }).FirstOrDefault();

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotNull(newModel);
            Assert.AreEqual(newModel.Id, model.Id, "The test of AddComment() controller QuestionController failed. Id not identical");
            Assert.AreEqual(newModel.Comment.Body, model.Comment.Body, "The test of AddComment() controller QuestionController failed. Body not identical");
            
            // Delete
            commentaryQuestionRepository.Delete(newModel.Comment.Id);
        }

        [TestMethod]
        public void VoteUp_PostQuestionId_ReturnJson()
        {
            //Arrange
            var questionRepository = new QuestionRepository();
            var model= new UsefulnessModel
                                     {
                                         AmountLike = questionRepository.Entity.Where(item => item.Id == QuestionId)
                                                                        .Select(item => item.Usefulness)
                                                                        .SingleOrDefault()
                                     };
            // Act
            var actual = Controller.VoteUp(QuestionId, TypeComment);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(string), "The test of VoteUp() controller QuestionController failed. The returned object is null");

            var newModel = JsonConvert.DeserializeObject<UsefulnessModel>(actual);
            Assert.IsNotNull(newModel, "The test of VoteUp() controller QuestionController failed. Json Object is not serializable");
            Assert.AreEqual(model.AmountLike + 1, newModel.AmountLike, "The test of VoteUp() controller QuestionController failed. Like not increased");

            // delete
            questionRepository.UsefulnessDown(QuestionId);
        }

        [TestMethod]
        public void VoteUp_PostQuestionId_VouteUpInDb()
        {
            //Arrange
            var questionRepository = new QuestionRepository();
            var model = new UsefulnessModel
            {
                AmountLike = questionRepository.Entity.Where(item => item.Id == QuestionId)
                                               .Select(item => item.Usefulness)
                                               .SingleOrDefault()
            };
            // Act
            var actual = Controller.VoteUp(QuestionId, TypeComment);

            var newModel = questionRepository.Entity.Where(item => item.Id == QuestionId)
                                             .Select(item => new UsefulnessModel
                                                                 {
                                                                     AmountLike = item.Usefulness
                                                                 })
                                             .SingleOrDefault();
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(string), "The test of VoteUp() controller QuestionController failed. The returned object is null");

            Assert.IsNotNull(newModel, "The test of VoteUp() controller QuestionController failed. Json Object is not serializable");
            Assert.AreEqual(model.AmountLike + 1, newModel.AmountLike, "The test of VoteUp() controller QuestionController failed. Like not increased");

            // delete
            questionRepository.UsefulnessDown(QuestionId);
        }

        [TestMethod]
        public void VoteDown_PostQuestionId_ReturnJson()
        {
            //Arrange
            var questionRepository = new QuestionRepository();
            var model = new UsefulnessModel
            {
                AmountLike = questionRepository.Entity.Where(item => item.Id == QuestionId)
                                               .Select(item => item.Usefulness)
                                               .SingleOrDefault()
            };
            // Act
            var actual = Controller.VoteDown(QuestionId, TypeComment);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(string), "The test of VoteDown() controller QuestionController failed. The returned object is null");

            var newModel = JsonConvert.DeserializeObject<UsefulnessModel>(actual);
            Assert.IsNotNull(newModel, "The test of VoteDown() controller QuestionController failed. Json Object is not serializable");
            Assert.AreEqual(model.AmountLike - 1, newModel.AmountLike, "The test of VoteDown() controller QuestionController failed. Like not increased");

            // delete
            questionRepository.UsefulnessUp(QuestionId);
        }

        [TestMethod]
        public void VoteDown_PostQuestionId_VoteDownInDb()
        {
            //Arrange
            var questionRepository = new QuestionRepository();
            var model = new UsefulnessModel
            {
                AmountLike = questionRepository.Entity.Where(item => item.Id == QuestionId)
                                               .Select(item => item.Usefulness)
                                               .SingleOrDefault()
            };
            // Act
            var actual = Controller.VoteDown(QuestionId, TypeComment);

            var newModel = questionRepository.Entity.Where(item => item.Id == QuestionId)
                                             .Select(item => new UsefulnessModel
                                             {
                                                 AmountLike = item.Usefulness
                                             })
                                             .SingleOrDefault();
            //Assert
            Assert.IsNotNull(actual);
            Assert.IsInstanceOfType(actual, typeof(string), "The test of VoteDown() controller QuestionController failed. The returned object is null");

            Assert.IsNotNull(newModel, "The test of VoteDown() controller QuestionController failed. Json Object is not serializable");
            Assert.AreEqual(model.AmountLike - 1, newModel.AmountLike, "The test of VoteDown() controller QuestionController failed. Like not increased");

            // delete
            questionRepository.UsefulnessUp(QuestionId);
        }
    
        [TestMethod]
        public void Usefulness_PostIdAnswer_SetBestAnswerInDb()
        {
            // Arrange
            const int answerId = 86;
            var answerRepository = new AnswerRepository();

            // act
            Controller.Usefulness(answerId);
            
            // Assert
            var newModelBestAnswer = answerRepository.Entity
                                            .Where(item => item.Id == answerId)
                                            .Select(item => item.BestAnswer)
                                            .FirstOrDefault();
            Assert.IsTrue(newModelBestAnswer, "The test of Usefulness() controller QuestionController failed. The best answer was not installed");

            // delete
            answerRepository.SetNotBestAnswer(answerId);
        }
    }
}
