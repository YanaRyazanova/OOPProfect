using System;
using System.Collections.Generic;
using System.Text;
using Domain.Functions;
using Infrastructure;
using NUnit.Framework;

namespace Tests
{
    //[TestFixture]
    //public class DomainTest
    //{
    //    [Test]
    //    public void TestLessonReminder()
    //    {
    //        while (true)
    //        {
    //            var actual = new LessonReminder(new Lesson(DateTime.Now.AddMinutes(10), "TestLesson")).Do();
    //            Assert.AreEqual("Через 10 начнется пара TestLesson", actual);
    //            break;
    //        }
    //    }

    //    [Test]
    //    public void TestScheduleSender()
    //    {
    //        var actual = new ScheduleSender("12:00 первая пара").Do();
    //        Assert.AreEqual("12:00 первая пара", actual);
    //    }

    //    [Test]
    //    public void TestDiningRoomIndicator()
    //    {
    //        var actual = new DiningRoomIndicator();
    //        actual.Increment();
    //        Assert.AreEqual(1, actual.VisitorsCount);
    //    }
    //}

    //public class DBTest
    //{
    //    [Test]
    //    public void DataBaseTest()
    //    {
    //        var dataBase = new DataBase().GetNearestLesson("ФТ-201");
            
    //    }
    //}
}