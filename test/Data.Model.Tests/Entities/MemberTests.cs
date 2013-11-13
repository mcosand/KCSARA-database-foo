using Kcsar.Database.Model;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
