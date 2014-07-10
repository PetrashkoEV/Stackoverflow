using System;
using System.Collections.Generic;

namespace Stackoverflow.Models.Questions
{
    public class QuestionModel
    {
        public long Id { get; set; }
        public UserModel User { get; set; }
        public int Usefulness { get; set; } // like
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime PublicationDate { get; set; }

        public List<CommentaryModel> QuestionComments { get; set; }
        public List<AnswerModel> Answers { get; set; }

        public string Answer { get; set; }
        public string Comment { get; set; }
    }
}