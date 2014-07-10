using System;
using System.Data.Entity;
using Stackoverflow.SqlContext.Entities;

namespace Stackoverflow.SqlContext.Concrete.Comments
{
    public class CommentaryQuestionRepository
    {
        private readonly StackoverflowEntities _dataContext = new StackoverflowEntities();

        public DbSet<CommentaryQuestion> Entity
        {
            get { return _dataContext.CommentaryQuestion; }
        }

        private CommentaryQuestion Find(long id)
        {
            return Entity.Find(id);
        }

        public void Add(long userId, long questionId, string body, DateTime publicationDate)
        {
            var item = new CommentaryQuestion();

            item.UserId = userId;
            item.QuestionId = questionId;
            item.Body = body;
            item.PublicationDate = publicationDate;

            Entity.Add(item);
            _dataContext.SaveChanges();
        }

        public void Edit(long id, long userId, long questionId, string body, DateTime publicationDate)
        {
            var item = Find(id);

            item.UserId = userId;
            item.QuestionId = questionId;
            item.Body = body;
            item.PublicationDate = publicationDate;

            _dataContext.SaveChanges();
        }

        public void Delete(long id)
        {
            var item = Entity.Find(id);
            Entity.Remove(item);
            _dataContext.SaveChanges();
        }
    }
}
