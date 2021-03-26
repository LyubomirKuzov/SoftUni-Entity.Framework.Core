using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Models
{
    public class Quiz
    {
        public Quiz()
        {
            this.Questions = new HashSet<Question>();
        }



        public int Id { get; set; }

        public string Title { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
