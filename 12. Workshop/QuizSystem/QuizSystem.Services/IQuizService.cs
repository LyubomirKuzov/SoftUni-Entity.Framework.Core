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
        void Add(string title);

        public QuizViewModel GetQuizById(int quizId);
    }
}
