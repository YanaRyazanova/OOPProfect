using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Bot.Domain.Functions;

namespace Tests
{
    [TestFixture]
    public class DomainTest
    {
        [Test]
        public void TestLessonReminder()
        {
            while (true)
            {
                var actual = new LessonReminder("ФТ-201").Do();
                if (actual == null) continue;
                Assert.NotNull(actual);
                break;
            }
        }
    }
}