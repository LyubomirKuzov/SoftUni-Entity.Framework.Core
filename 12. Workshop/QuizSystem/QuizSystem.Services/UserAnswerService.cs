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



        public void AddUserAnswer(string username, int questionId, int answerId)
        {
            var userId = this.dbContext.Users.Where(x => x.UserName == username)
                .Select(x => x.Id)
                .FirstOrDefault();

            var userAnswer = this.dbContext.UserAnswers.FirstOrDefault(x => x.IdentityUserId == userId
                                                                       && x.QuestionId == questionId);

            userAnswer.AnswerId = answerId;

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
                    AnswerId = item.AnswerId
                };

                userAnswers.Add(userAnswer);
            }

            this.dbContext.UserAnswers.AddRange(userAnswers);

            this.dbContext.SaveChanges();
        }

        public int UserResult(string username, int quizId)
        {
            var userId = this.dbContext.Users.Where(x => x.UserName == username)
                .Select(x => x.Id)
                .FirstOrDefault();

            var totalPoints = dbContext.UserAnswers
                .Where(x => x.IdentityUserId == userId && x.Question.QuizId == quizId)
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
