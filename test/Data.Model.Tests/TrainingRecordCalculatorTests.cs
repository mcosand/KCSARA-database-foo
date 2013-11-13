using Kcsar.Database.Model;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Internal.Data.Model.Tests
{
  [TestFixture]
  public class TrainingRecordCalculatorTests
  {
    [Test]
    public void RewardTraining_1()
    {
      InMemoryDbSet<ComputedTrainingAward> awards = new InMemoryDbSet<ComputedTrainingAward>();

      Mock<IKcsarContext> mockDB = new Mock<IKcsarContext>(MockBehavior.Strict);
      mockDB.SetupGet(f => f.ComputedTrainingAwards).Returns(awards);

      TestCalculator calc = new TestCalculator(mockDB.Object);

      Member me = new Member();
      Dictionary<Guid, TrainingCourse> courses = new Dictionary<Guid,TrainingCourse>();
      Dictionary<Guid, ComputedTrainingAward> myAwards = new Dictionary<Guid,ComputedTrainingAward>();
      TrainingRule rule = new TrainingRule();
      DateTime? completed = DateTime.Today;
      DateTime? expiry = DateTime.Today.AddMonths(12);
      string newString = Guid.NewGuid().ToString();

      calc.Test_RewardTraining(me, courses, myAwards, rule, completed, expiry, newString);
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
  }
}
