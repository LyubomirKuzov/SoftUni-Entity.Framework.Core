using Newtonsoft.Json;
using QuizSystem.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.ConsoleUI.Models
{
    public class JsonImportService : IJsonImportService
    {
        private readonly IQuizService quizService;
        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;

        public JsonImportService(IQuizService quizService, IQuestionService questionService, IAnswerService answerService)
        {
            this.quizService = quizService;
            this.questionService = questionService;
            this.answerService = answerService;
        }

        public void Import(string fileName, string quizName)
        {
            var json = File.ReadAllText(fileName);
            var questions = JsonConvert.DeserializeObject<IEnumerable<JsonQuestion>>(json);

            var quizId = quizService.Add(quizName);

            foreach (var q in questions)
            {
                var questionId = questionService.Add(q.Question, quizId);

                foreach (var a in q.Answers)
                {
                    answerService.Add(a.Answer, a.Correct ? 1 : 0, a.Correct, questionId);
                }
            }
        }
    }
}
