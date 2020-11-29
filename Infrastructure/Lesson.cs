using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class Lesson
    {
        public readonly DateTime time;
        public readonly string name;

        public Lesson(DateTime time_, string name_)
        {
            time = time_;
            name = name_;
        }
    }
}
