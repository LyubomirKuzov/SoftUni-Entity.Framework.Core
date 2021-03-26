using QuizSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Services
{
    public interface IQuizService
    {
        int Add(string title);

        public QuizViewModel GetQuizById(int quizId);

        public IEnumerable<UserQuizViewModel> GetQuizesByUserName(string username);

        void StartQuiz(string username, int quizId);
    }
}
