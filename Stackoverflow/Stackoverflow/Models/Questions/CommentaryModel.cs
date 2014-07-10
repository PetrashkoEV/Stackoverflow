using System;

namespace Stackoverflow.Models.Questions
{
    public class CommentaryModel
    {
        public long Id { get; set; }
        public UserModel User { get; set; }
        public string Body { get; set; }
        public DateTime PublicationDate { get; set; }
    }
}