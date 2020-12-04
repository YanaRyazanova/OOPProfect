using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Infrastructure
{
    public class Lesson: ITuple
    {
        public readonly DateTime time;
        public readonly string name;

        public Lesson(DateTime time_, string name_)
        {
            time = time_;
            name = name_;
        }

        public object? this[int index]
        {
            get
            {
                return index switch
                {
                    0 => time,
                    1 => name,
                    _ => null
                };
            }
        }

        public int Length { get => 2; }
    }
}
