using System;
using System.Data.Entity;
using Stackoverflow.SqlContext.Entities;

namespace Stackoverflow.SqlContext.Concrete.Comments
{
    public class CommentaryAnswerRepository
    {
        private readonly StackoverflowEntities _dataContext = new StackoverflowEntities();

        public DbSet<CommentaryAnswer> Entity
        {
            get { return _dataContext.CommentaryAnswer; }
        }

        private CommentaryAnswer Find(long id)
        {
            return Entity.Find(id);
        }

        public void Add(long userId, long answerId, string body, DateTime publicationDate)
        {
            var item = new CommentaryAnswer();

            item.UserId = userId;
            item.AnswerId = answerId;
            item.Body = body;
            item.PublicationDate = publicationDate;

            Entity.Add(item);
            _dataContext.SaveChanges();
        }

        public void Edit(long id, long userId, long answerId, string body, DateTime publicationDate)
        {
            var item = Find(id);

            item.UserId = userId;
            item.AnswerId = answerId;
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
