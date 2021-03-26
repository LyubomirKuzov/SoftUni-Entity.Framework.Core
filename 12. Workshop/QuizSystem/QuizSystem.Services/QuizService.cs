using QuizSystem.Data;
using QuizSystem.Models;
using QuizSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuizSystem.Services.Models.Enumerations;

namespace QuizSystem.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext dbContext;



        public QuizService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public int Add(string title)
        {
            var quiz = new Quiz()
            {
                Title = title
            };

            this.dbContext.Quizzes.Add(quiz);

            this.dbContext.SaveChanges();

            return quiz.Id;
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

        public IEnumerable<UserQuizViewModel> GetQuizesByUserName(string username)
        {
            var quizzes = dbContext.Quizzes
                .Select(q => new UserQuizViewModel()
                {
                    QuizId = q.Id,
                    Title = q.Title
                })
                .ToList();

            foreach (var q in quizzes)
            {
                var questionsCount = dbContext.UserAnswers
                    .Count(ua => ua.IdentityUser.UserName == username && ua.Question.QuizId == q.QuizId);

                if (questionsCount == 0)
                {
                    q.QuizStatus = QuizStatus.NotStarted;
                    continue;
                }

                var answeredQuestions = dbContext.UserAnswers
                    .Count(ua => ua.IdentityUser.UserName == username
                          && ua.Question.QuizId == q.QuizId
                          && ua.AnswerId.HasValue);

                if (answeredQuestions == questionsCount)
                {
                    q.QuizStatus = QuizStatus.Finished;
                }

                else
                {
                    q.QuizStatus = QuizStatus.InProgress;
                }
            }

            return quizzes;
        }

        public void StartQuiz(string username, int quizId)
        {
            if (this.dbContext.UserAnswers.Any(a => a.IdentityUser.UserName == username
                && a.Question.QuizId == quizId))
            {
                return;
            }

            var userId = this.dbContext.Users.Where(x => x.UserName == username)
                .Select(x => x.Id)
                .FirstOrDefault();

            var questions = dbContext.Questions.Select(q => new
            {
                q.Id
            })
            .ToList();

            foreach (var question in questions)
            {
                dbContext.UserAnswers.Add(new UserAnswer()
                {
                    AnswerId = null,
                    IdentityUserId = userId,
                    QuestionId = question.Id
                });
            }

            dbContext.SaveChanges();
        }
    }
}
