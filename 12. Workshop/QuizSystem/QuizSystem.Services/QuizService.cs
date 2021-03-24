using QuizSystem.Data;
using QuizSystem.Models;
using QuizSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QuizSystem.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext dbContext;



        public QuizService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public void Add(string title)
        {
            var quiz = new Quiz()
            {
                Title = title
            };

            this.dbContext.Quizzes.Add(quiz);

            this.dbContext.SaveChanges();
        }

        public QuizViewModel GetQuizById(int quizId)
        {
            var quiz = this.dbContext.Quizzes
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .FirstOrDefault(q => q.Id == quizId);

            var quizViewModel = new QuizViewModel()
            {
                Title = quiz.Title,
                Questions = quiz.Questions
                    .Select(q => new QuestionViewModel()
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Answers = q.Answers
                            .Select(a => new AnswerViewModel()
                            {
                                Id = a.Id,
                                Title = a.Title
                            })
                    })
            };

            return quizViewModel;
        }
    }
}
