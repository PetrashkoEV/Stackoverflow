using System;
using System.Collections.Generic;

namespace Stackoverflow.Models.Questions
{
    public class AnswerModel
    {
        public long Id { get; set; }
        public UserModel User { get; set; }
        public int Usefulness { get; set; } // amount like
        public string Body { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool BestAnswer { get; set; } 

        public List<CommentaryModel> Comments { get; set; }
    }
}