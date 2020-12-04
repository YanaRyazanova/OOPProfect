using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using MyDomain.Functions;

namespace Tests
{
    [TestFixture]
    public class DomainTest
    {
        [Test]
        public void TestLessonReminder()
        {
            //while (true)
            //{
            //    var actual = new LessonReminder("ФТ-202").Do();
            //    if (actual == null) continue;
            //    Assert.NotNull(actual);
            //    break;
            //}
        }

        [Test]
        public void TestScheduleSender()
        {
            //var actual = new ScheduleSender("ФТ-202").Do();
            //Assert.NotNull(actual);
        }

        [Test]
        public void TestDiningRoomIndicator()
        {
            var actual = new DiningRoomIndicator();
            actual.Increment();
            Assert.AreEqual(1, actual.VisitorsCount);
        }
    }
}