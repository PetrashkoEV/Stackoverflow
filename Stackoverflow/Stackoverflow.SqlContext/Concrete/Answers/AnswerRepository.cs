using System;
using System.Data.Entity;
using System.Linq;
using Stackoverflow.SqlContext.Entities;

namespace Stackoverflow.SqlContext.Concrete.Answers
{
    public class AnswerRepository
    {
        private readonly StackoverflowEntities _dataContext = new StackoverflowEntities();

        public DbSet<Answer> Entity
        {
            get { return _dataContext.Answer; }
        }

        private Answer Find(long id)
        {
            return Entity.Find(id);
        }

        public void Add(long userId, long questionId, int usefulness, string body, DateTime publicationDate)
        {
            var item = new Answer();
            item.UserId = userId;
            item.QuestionId = questionId;
            item.Usefulness = usefulness;
            item.Body = body;
            item.PublicationDate = publicationDate;

            Entity.Add(item);
            _dataContext.SaveChanges();
        }

        public void Edit(long id, string body)
        {
            var item = Find(id);
            item.Body = body;
            _dataContext.SaveChanges();
        }

        public void Delete(long id)
        {
            var comments = _dataContext.CommentaryAnswer.Where(comment => comment.AnswerId == id).ToList();
            foreach (var commentary in comments)
            {
                _dataContext.CommentaryAnswer.Remove(commentary);
            }

            var item = Find(id);
            Entity.Remove(item);
            _dataContext.SaveChanges();
        }
    
        public void UsefulnessUp (long id)
        {
            var item = Find(id);
            item.Usefulness++;
            _dataContext.SaveChanges();
        }

        public void UsefulnessDown(long id)
        {
            var item = Find(id);
            item.Usefulness--;
            _dataContext.SaveChanges();
        }
    
        public void SetBestAnswer(long id)
        {
            var item = Find(id);
            item.BestAnswer = true;
            _dataContext.SaveChanges();
        }

        public void SetNotBestAnswer(long id)
        {
            var item = Find(id);
            item.BestAnswer = false;
            _dataContext.SaveChanges();
        }
    
        public void UpdateBestAnswer (long id)
        {
            var questionId = Entity.Where(answer => answer.Id == id)
                                    .Select(answer => answer.QuestionId)
                                    .FirstOrDefault();

            var oldBestAnswerId = Entity.Where(answer => answer.QuestionId == questionId)
                            .Where(answer => answer.BestAnswer)
                            .Select(answer => answer.Id)
                            .FirstOrDefault();

            var oldBestAnswer = Find(oldBestAnswerId);
            oldBestAnswer.BestAnswer = false;

            var item = Find(id);
            item.BestAnswer = true;
            _dataContext.SaveChanges();
        }
    }
}
