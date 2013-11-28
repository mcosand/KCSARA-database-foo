using System;
using Kcsar.Database.Model;
using NUnit.Framework;

namespace Internal.Data.Model.Tests.Entities
{    
    public class MemberTests : EntityTestFixture<Member>
    {
        [Test]
        public void Instantiate()
        {
            Member m = new Member();
            Assert.AreEqual(DateTime.Today, m.WacLevelDate, "wacleveldate");
            Assert.AreNotEqual(Guid.Empty, m.Id, "id");
        }
    }
}
