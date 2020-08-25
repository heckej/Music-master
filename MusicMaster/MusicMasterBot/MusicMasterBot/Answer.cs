using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMasterBot
{
    public class Answer
    {
        public Answer(Question question)
        {
            Question = question;
        }

        public Answer() { }

        public Question Question { get; private set; }
        public string Text { get; set; }
        public string Speak { get; set; }
    }
}
