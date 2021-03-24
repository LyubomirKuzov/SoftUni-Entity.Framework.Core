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
    public class UserAnswerService : IUserAnswerService
    {
        private readonly ApplicationDbContext dbContext;



        public UserAnswerService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public void AddUserAnswer(string userId, int quizId, int questionId, int answerId)
        {
            var userAnswer = new UserAnswer()
            {
                IdentityUserId = userId,
                QuizId = quizId,
                QuestionId = questionId,
                AnswerId = answerId
            };

            dbContext.UserAnswers.Add(userAnswer);

            dbContext.SaveChanges();
        }

        public void BulkAddUserAnswer(QuizInputModel quizInputModel)
        {
            var userAnswers = new List<UserAnswer>();

            foreach (var item in quizInputModel.Questions)
            {
                var userAnswer = new UserAnswer()
                {
                    IdentityUserId = quizInputModel.UserId,
                    QuizId = quizInputModel.QuizId,
                    AnswerId = item.AnswerId,
                    QuestionId = item.QuestionId
                };

                userAnswers.Add(userAnswer);
            }

            this.dbContext.UserAnswers.AddRange(userAnswers);

            this.dbContext.SaveChanges();
        }

        public int UserResult(string userId, int quizId)
        {
            var totalPoints = dbContext.Quizzes
                .Include(x => x.Questions)
                .ThenInclude(x => x.Answers)
                .ThenInclude(x => x.UserAnswers)
                .Where(x => x.Id == quizId && x.UserAnswers.Any(x => x.IdentityUserId == userId))
                .SelectMany(x => x.UserAnswers)
                .Where(x => x.Answer.IsCorrect)
                .Sum(x => x.Answer.Points);

            return totalPoints;

            //var userAnswers = this.dbContext.UserAnswers
            //    .Where(x => x.IdentityUserId == userId && x.QuizId == quizId)
            //    .ToList();

            //int? totalPoints = 0;

            //foreach (var ua in userAnswers)
            //{
            //    var points = originalQuiz.Questions
            //        .FirstOrDefault(x => x.Id == ua.QuestionId)
            //        .Answers
            //        .Where(x => x.IsCorrect)
            //        .FirstOrDefault(x => x.Id == ua.AnswerId)
            //        ?.Points;

            //    totalPoints += points;
            //}
        }
    }
}
