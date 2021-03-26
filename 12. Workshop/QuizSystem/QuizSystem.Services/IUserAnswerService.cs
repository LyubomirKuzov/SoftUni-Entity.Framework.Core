using QuizSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Services
{
    public interface IUserAnswerService
    {
        void AddUserAnswer(string userId, int questionId, int answerId);

        public void BulkAddUserAnswer(QuizInputModel quizInputModel);

        public int UserResult(string userId, int quizId);
    }
}
