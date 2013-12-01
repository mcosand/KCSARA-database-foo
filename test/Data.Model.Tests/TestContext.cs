/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Internal.Data.Model.Tests
{
  using System;
  using System.Data.Entity;
  using Internal.Common;
  using Kcsar.Database.Model;

  public class TestContext : IKcsarContext
  {
    public IDbSet<Animal> Animals { get; private set; }
    public IDbSet<AnimalOwner> AnimalOwners { get; private set; }
    public IDbSet<AnimalMission> AnimalMissions { get; private set; }
    public IDbSet<Mission> Missions { get; private set; }
    public IDbSet<MissionDetails> MissionDetails { get; private set; }
    public IDbSet<MissionLog> MissionLog { get; private set; }
    public IDbSet<MissionRoster> MissionRosters { get; private set; }
    public IDbSet<MissionGeography> MissionGeography { get; private set; }
    public IDbSet<Member> Members { get; private set; }
    public IDbSet<PersonAddress> PersonAddress { get; private set; }
    public IDbSet<PersonContact> PersonContact { get; private set; }
    public IDbSet<MemberUnitDocument> MemberUnitDocuments { get; private set; }
    public IDbSet<Subject> Subjects { get; private set; }
    public IDbSet<SubjectGroup> SubjectGroups { get; private set; }
    public IDbSet<SubjectGroupLink> SubjectGroupLinks { get; private set; }
    public IDbSet<Training> Trainings { get; private set; }
    public IDbSet<TrainingAward> TrainingAward { get; private set; }
    public IDbSet<TrainingCourse> TrainingCourses { get; private set; }
    public IDbSet<Document> Documents { get; private set; }
    public IDbSet<TrainingRoster> TrainingRosters { get; private set; }
    public IDbSet<TrainingRule> TrainingRules { get; private set; }
    public IDbSet<SarUnit> Units { get; private set; }
    public IDbSet<UnitApplicant> UnitApplicants { get; private set; }
    public IDbSet<UnitMembership> UnitMemberships { get; private set; }
    public IDbSet<UnitStatus> UnitStatusTypes { get; private set; }
    public IDbSet<UnitDocument> UnitDocuments { get; private set; }
    public IDbSet<ComputedTrainingAward> ComputedTrainingAwards { get; private set; }
    public IDbSet<TrainingExpirationSummary> TrainingExpirationSummaries { get; private set; }
    public IDbSet<CurrentMemberIds> CurrentMemberIds { get; private set; }
    public IDbSet<xref_county_id> xref_county_id { get; private set; }
    public IDbSet<SensitiveInfoAccess> Test_SensitiveInfo { get; private set; }

    public TestContext()
    {
      this.Members = new InMemoryDbSet<Member>();
      this.TrainingCourses = new InMemoryDbSet<TrainingCourse>();
      this.ComputedTrainingAwards = new InMemoryDbSet<ComputedTrainingAward>();
      this.TrainingAward = new InMemoryDbSet<TrainingAward>();
      this.TrainingRules = new InMemoryDbSet<TrainingRule>();

      this.Missions = new InMemoryDbSet<Mission>();
      this.MissionRosters = new InMemoryDbSet<MissionRoster>();

      this.Test_SensitiveInfo = new InMemoryDbSet<SensitiveInfoAccess>();
    }

    public Func<int> saveChangesBody = () => 0;
    public int SaveChanges()
    {
      return saveChangesBody();
    }


    public Action<SensitiveInfoAccess> recordAccessBody = null;
    public void RecordSensitiveAccess(SensitiveInfoAccess record)
    {
      if (this.recordAccessBody != null)
      {
        this.recordAccessBody(record);
      }
      else
      {
        this.Test_SensitiveInfo.Add(record);
      }
    }
  }
}
