using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Kcsar.Database.Model;
using NUnit.Framework;

namespace Internal.Data.Model.Tests
{
    [TestFixture]
    public class KcsarContextTests
    {
        string connectionString = null;
        [TestFixtureSetUp]
        public void Setup()
        {
            this.connectionString = CreateTestDatabase();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            using (var db = GetContext())
            {
                db.Database.Delete();
            }
        }

        [Test]
        public void LogNewEntity()
        {
            DateTime now = MarkTime();

            Member member;
            using (var db = GetContext())
            {
                member = new Member { FirstName = "Test", LastName = "User" };
                db.Members.Add(member);
                db.SaveChanges();
            }

            using (var db = GetContext())
            {
                var logs = PullLogs(now, true);
                Assert.AreEqual(1, logs.Length, "log count");
                Assert.AreEqual("Members", logs[0].Collection, "collection");
                Assert.AreEqual(EntityState.Added.ToString(), logs[0].Action, "action");
                Assert.AreEqual(member.GetReportHtml(), logs[0].Comment, "comment");
            }
        }

        [Test]
        public void LogUpdateEntity()
        {

            Member member;
            using (var db = GetContext())
            {
                member = new Member { FirstName = "tTest", LastName = "User" };
                db.Members.Add(member);
                db.SaveChanges();
            }

            DateTime now = MarkTime();

            using (var db = GetContext())
            {
                member = db.Members.Find(member.Id);
                member.FirstName = "Test";
                member.WacLevel = WacLevel.Field;
                db.SaveChanges();
            }

            using (var db = GetContext())
            {
                var logs = PullLogs(now, true);
                Assert.AreEqual(1, logs.Length, "log count");
                Assert.AreEqual("Members", logs[0].Collection, "collection");
                Assert.AreEqual(EntityState.Modified.ToString(), logs[0].Action, "action");
                Assert.AreEqual("FirstName: tTest => Test<br/>WacLevel: None => Field<br/>", logs[0].Comment, "comment");
            }
        }

        [Test]
        public void LogDeleteEntity()
        {
            string reportHtml;
            Member member;
            using (var db = GetContext())
            {
                member = new Member { FirstName = "tTest", LastName = "User" };
                db.Members.Add(member);
                db.SaveChanges();
                reportHtml = member.GetReportHtml();
            }

            DateTime now = MarkTime();

            using (var db = GetContext())
            {
                member = db.Members.Find(member.Id);
                db.Members.Remove(member);
                db.SaveChanges();
            }

            using (var db = GetContext())
            {
                var logs = PullLogs(now, true);
                Assert.AreEqual(1, logs.Length, "log count");
                Assert.AreEqual("Members", logs[0].Collection, "collection");
                Assert.AreEqual(EntityState.Deleted.ToString(), logs[0].Action, "action");
                Assert.AreEqual(reportHtml, logs[0].Comment, "comment");
            }
        }

        [Test]
        public void InvalidObjectsNotSaved()
        {
            int memberCount;
            int logCount;
            DateTime start = DateTime.Now;
            using (var db = GetContext())
            {
                memberCount = db.Members.Count();
                logCount = db.GetLog(start).Length;
            }

            using (var innerDb = GetContext())
            {
                Member m = new Member();
                List<ValidationResult> results = new List<ValidationResult>();
                Assert.IsFalse(Validator.TryValidateObject(m, new ValidationContext(m, null, null), results, true));
                innerDb.Members.Add(m);
                Assert.Throws<DbEntityValidationException>(() => innerDb.SaveChanges());
            }

            using (var db = GetContext())
            {
                Assert.AreEqual(memberCount, db.Members.Count(), "member count");
                Assert.AreEqual(logCount, db.GetLog(start).Length, "log count");
            }
        }

        private static void DumpLogs(AuditLog[] logs)
        {
            Console.WriteLine(string.Join("\n", logs.Select(f => string.Format("{0}: {1}", f.Changed, f.Comment))));
        }

        private static DateTime MarkTime()
        {
            Thread.Sleep(1000);
            DateTime result = DateTime.Now;
            while (result.Second == DateTime.Now.Second)
            {
              Thread.Sleep(100);
            }
            return result;
        }

        private AuditLog[] PullLogs(DateTime since, bool dump)
        {            
            using (var db = GetContext())
            {
                var logs = db.GetLog(since);
                if (dump) DumpLogs(logs);
                return logs;
            }
        }

        private static string CreateTestDatabase()
        {
            var path = string.Format(@"{0}\{1}.mdf", System.IO.Directory.GetCurrentDirectory(), Guid.NewGuid());

            string connString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + path + @";Integrated Security=True;Connect Timeout=30";
            Console.WriteLine("Using local database: " + path);

            Stopwatch timer = new Stopwatch();
            timer.Start();

            using (var context = new KcsarContext(connString))
            {
                var initializer = new MigrateDbToLatestInitializerConnString<KcsarContext, Kcsar.Database.Model.Migrations.Configuration>();
                initializer.InitializeDatabase(context);
            }

            timer.Stop();
            Console.WriteLine("Database setup took {0}s", timer.Elapsed.TotalSeconds);

            return connString;
        }

        private KcsarContext GetContext()
        {
            KcsarContext db = new KcsarContext(this.connectionString);
            
//            db.Database.Log = Console.Write;
            return db;
        }
    }
}
