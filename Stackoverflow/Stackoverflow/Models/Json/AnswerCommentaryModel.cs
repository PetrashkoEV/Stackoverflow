using Stackoverflow.Models.Questions;

namespace Stackoverflow.Models.Json
{
    public class AnswerCommentaryModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public CommentaryModel Comment { get; set; }
    }
}