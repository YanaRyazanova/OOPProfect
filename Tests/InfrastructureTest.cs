using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Infrastructure.SQL;
using Infrastructure.Csv;
using Infrastructure;

namespace Tests
{
    [TestFixture]
    class InfrastructureTest
    {
        [Test]
        public void PeopleParserSQLTest()
        {
            var parser = new PeopleParserSql(new DBNameProvider());
            parser.AddNewUser("test", "tg");
            parser.ChangeGroup("test", "testGroup");
            Assert.AreEqual("testGroup", parser.GetGroupFromId("test"));
            Assert.AreEqual("0", parser.GetStateFromId("test"));
            parser.EvaluateState("test");
            Assert.AreEqual("1", parser.GetStateFromId("test"));
            parser.ChangeState("test", "4");
            Assert.AreEqual("4", parser.GetStateFromId("test"));
            Assert.AreEqual("tg", parser.GetPlatformFromId("test"));
        }

        [Test]
        public void PeopleParserCSVTest()
        {
            var parser = new PeopleParserCsv(new DBNameProvider());
            parser.AddNewUser("test", "vk", "0");
            parser.ChangeGroup("test", "artemiq rogov");
            Assert.AreEqual("artemiq rogov", parser.GetGroupFromId("test"));
            Assert.AreEqual("0", parser.GetStateFromId("test"));
            parser.EvaluateState("test");
            Assert.AreEqual("1", parser.GetStateFromId("test"));
            parser.ChangeState("test", "150");
            Assert.AreEqual("150", parser.GetStateFromId("test"));
            Assert.AreEqual("vk", parser.GetPlatformFromId("test"));
        }
    }
}
