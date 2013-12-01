namespace Internal.Data.Model.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Internal.Common;
  using Kcsar.Database.Model;
  using Moq;
  using NUnit.Framework;

  [TestFixture]
  public class TrainingRecordCalculatorTests
  {
    [Test]
    public void Calculate_MissionHours()
    {
      TestContext db = new TestContext();
      var me = db.Members.Add(new Member());

      Func<int, double, string, string, MissionRoster> createMission = (daysAgo, hours, role, type) =>
      {
        Mission m = db.Missions.Add(new Mission { StartTime = DateTime.Now.AddDays(-daysAgo).AddHours(-1), MissionType = type });
        MissionRoster r = db.MissionRosters.Add(new MissionRoster
        {
          Person = me,
          Mission = m,
          TimeIn = DateTime.Now.AddDays(-daysAgo),
          TimeOut = DateTime.Now.AddDays(-daysAgo).AddHours(hours),
          InternalRole = role
        });
        return r;
      };

      createMission(15, 3.5, "field", "search");
      createMission(63, 8.0, "field", "resecue");
      var satisfies = createMission(80, 4, "field", "urban,search");
      createMission(100, 23, "field", "search");

      var course = db.TrainingCourses.Add(new TrainingCourse { DisplayName = "Target", FullName = "Target", ValidMonths = 36 });

      db.TrainingRules.Add(new TrainingRule { RuleText = string.Format("Mission(12:%:36)>{0}", course.Id) });

      TrainingRecordCalculator calc = new TrainingRecordCalculator(db);
      var result = calc.Calculate(new[] { me }, DateTime.Now);

      Assert.AreEqual(1, result.Count, "awarded records");
      var record = db.ComputedTrainingAwards.Single();
      Assert.AreEqual(satisfies.TimeIn.Value.AddMonths(course.ValidMonths.Value), record.Expiry.Value, "go back until we have the hours, add course months");

    }

    #region RewardTraining
    [Test]
    public void RewardTraining_CourseNotFound()
    {
      var tester = RewardTrainingTestData.Create();
      tester.NewString = Guid.NewGuid().ToString();

      TestCalculator calc = new TestCalculator(tester.MockDB.Object);

      Assert.Throws<InvalidOperationException>(() => tester.RunRewardTraining());
    }

    [Test]
    public void RewardTraining_CourseWithExpiryGivesTwoCourses()
    {
      var tester = RewardTrainingTestData.Create();
      var source = tester.AddCourse(new TrainingCourse { DisplayName = "A", FullName = "Composite Source A", ValidMonths = 24 });
      var resultA = tester.AddCourse(new TrainingCourse { DisplayName = "B", FullName = "Component Course B" });
      var resultB = tester.AddCourse(new TrainingCourse { DisplayName = "C", FullName = "Component Course C" });

      tester.NewString = string.Format("{0}+{1}", resultA.Id, resultB.Id);

      bool result = tester.RunRewardTraining();

      Assert.IsTrue(result, "should have indicated a new record");
      Assert.AreEqual(2, tester.DatabaseRecords.Count(), "expected awards count");
      Assert.IsTrue(tester.DatabaseRecords.Any(f => f.Completed == tester.Completed && f.Course == resultA && f.Member == tester.Member && f.Expiry == tester.Expiry));
      Assert.IsTrue(tester.DatabaseRecords.Any(f => f.Completed == tester.Completed && f.Course == resultB && f.Member == tester.Member && f.Expiry == tester.Expiry));
    }

    [Test]
    public void RewardTraining_UseTargetsDefaultExpiry()
    {
      int months = 35;
      var tester = RewardTrainingTestData.Create();
      var target = tester.AddCourse(new TrainingCourse { DisplayName = "Target", FullName = "Target Course With Expiry", ValidMonths = months });

      tester.NewString = string.Format("{0}:default", target.Id);

      bool result = tester.RunRewardTraining();

      Assert.IsTrue(result, "should have added a record");
      Assert.AreEqual(1, tester.DatabaseRecords.Count(), "expected awards count");
      var record = tester.DatabaseRecords.Single();
      Assert.AreEqual(tester.Completed.Value.AddMonths(months), record.Expiry, "resulting expiry");
    }

    [Test]
    public void RewardTraining_UseTargetsDefaultNoExpiry()
    {
      var tester = RewardTrainingTestData.Create();
      var target = tester.AddCourse(new TrainingCourse { DisplayName = "Target", FullName = "Target" });

      tester.NewString = string.Format("{0}:default", target.Id);

      bool result = tester.RunRewardTraining();

      Assert.IsTrue(result, "should have added record");
      var record = tester.DatabaseRecords.Single();
      Assert.IsFalse(record.Expiry.HasValue, "expiration");
    }

    [Test]
    public void RewardTraining_UseRuleExpiry()
    {
      int customMonths = 16;
      var tester = RewardTrainingTestData.Create();
      var target = tester.AddCourse(new TrainingCourse { DisplayName = "Target", FullName = "Target", ValidMonths = 13 });

      tester.NewString = string.Format("{0}:{1}", target.Id, customMonths);

      bool result = tester.RunRewardTraining();

      Assert.IsTrue(result, "should have added record");
      var record = tester.DatabaseRecords.Single();
      Assert.AreEqual(tester.Completed.Value.AddMonths(customMonths), record.Expiry, "reward should use expiry specified by rule");
    }

    [Test]
    public void RewardTraining_UpdatesExisting()
    {
      int months = 24;
      var tester = RewardTrainingTestData.Create();
      var target = tester.AddCourse(new TrainingCourse { DisplayName = "Target", FullName = "Target Course", ValidMonths = months });
      var existing = new ComputedTrainingAward
      {
        Course = target,
        Member = tester.Member,
        Expiry = DateTime.Today.AddDays(7),
        Completed = DateTime.Today.AddMonths(-months).AddDays(7)
      };

      tester.DatabaseRecords.Add(existing);
      tester.myAwards = tester.DatabaseRecords.ToDictionary(f => f.Course.Id, f => f);
      tester.NewString = target.Id.ToString();

      bool result = tester.RunRewardTraining();

      Assert.IsTrue(result, "should have updated record");
      var record = tester.DatabaseRecords.Single();
      Assert.AreEqual(tester.Expiry, record.Expiry, "expirt");


    }

    public class RewardTrainingTestData
    {
      public DateTime? Completed = DateTime.Today;
      public DateTime? Expiry = DateTime.Today.AddMonths(12);
      public TrainingRule TrainingRule = new TrainingRule();
      public Dictionary<Guid, ComputedTrainingAward> myAwards = new Dictionary<Guid, ComputedTrainingAward>();
      public Dictionary<Guid, TrainingCourse> Courses = new Dictionary<Guid, TrainingCourse>();
      public string NewString = string.Empty;
      public Member Member = new Member();

      public InMemoryDbSet<ComputedTrainingAward> DatabaseRecords = new InMemoryDbSet<ComputedTrainingAward>();
      public Mock<IKcsarContext> MockDB = new Mock<IKcsarContext>(MockBehavior.Strict);

      public static RewardTrainingTestData Create()
      {
        return new RewardTrainingTestData();
      }

      public RewardTrainingTestData()
      {
        this.MockDB.SetupGet(f => f.ComputedTrainingAwards).Returns(this.DatabaseRecords);
      }
      public bool RunRewardTraining()
      {
        TestCalculator calc = new TestCalculator(this.MockDB.Object);
        return calc.Test_RewardTraining(Member, Courses, myAwards, TrainingRule, Completed, Expiry, NewString);
      }

      public TrainingCourse AddCourse(TrainingCourse course)
      {
        this.Courses.Add(course.Id, course);
        return course;
      }
    }

    public class TestCalculator : TrainingRecordCalculator
    {
      public TestCalculator(IKcsarContext db)
        : base(db)
      {
      }

      public bool Test_RewardTraining(Member m, Dictionary<Guid, TrainingCourse> courses, Dictionary<Guid, ComputedTrainingAward> awards, TrainingRule rule, DateTime? completed, DateTime? expiry, string newAwardsString)
      {
        return this.RewardTraining(m, courses, awards, rule, completed, expiry, newAwardsString);
      }
    }
    #endregion
  }
}
