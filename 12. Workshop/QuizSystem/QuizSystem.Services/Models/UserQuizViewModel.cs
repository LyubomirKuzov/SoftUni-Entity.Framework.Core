using QuizSystem.Services.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Services.Models
{
    public class UserQuizViewModel
    {
        public int QuizId { get; set; }

        public string Title { get; set; }

        public QuizStatus QuizStatus { get; set; }
    }
}
