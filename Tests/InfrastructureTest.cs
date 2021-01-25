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
            Assert.AreEqual(UserStates.NotRegister, parser.GetStateFromId("test"));
            parser.EvaluateState("test");
            Assert.AreEqual(UserStates.RegisterInProcess, parser.GetStateFromId("test"));
            parser.ChangeState("test", UserStates.AddingLink);
            Assert.AreEqual(UserStates.AddingLink, parser.GetStateFromId("test"));
            Assert.AreEqual("tg", parser.GetPlatformFromId("test"));
        }

        [Test]
        public void PeopleParserCSVTest()
        {
            var parser = new PeopleParserCsv(new DBNameProvider());
            parser.AddNewUser("test", "vk", UserStates.NotRegister);
            parser.ChangeGroup("test", "artemiq rogov");
            Assert.AreEqual("artemiq rogov", parser.GetGroupFromId("test"));
            Assert.AreEqual(UserStates.NotRegister, parser.GetStateFromId("test"));
            parser.EvaluateState("test");
            Assert.AreEqual(UserStates.RegisterInProcess, parser.GetStateFromId("test"));
            parser.ChangeState("test", UserStates.RegisterInProcess);
            Assert.AreEqual(UserStates.RegisterInProcess, parser.GetStateFromId("test"));
            Assert.AreEqual("vk", parser.GetPlatformFromId("test"));
        }
    }
}
