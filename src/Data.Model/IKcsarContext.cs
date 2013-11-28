/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Model
{
  using System.Data.Entity;

  public interface IKcsarContext
  {
    IDbSet<Animal> Animals { get; }
    IDbSet<AnimalMission> AnimalMissions { get; }
    IDbSet<AnimalOwner> AnimalOwners { get; }
    IDbSet<Mission> Missions { get; }
    IDbSet<MissionDetails> MissionDetails { get; }
    IDbSet<MissionLog> MissionLog { get; }
    IDbSet<MissionRoster> MissionRosters { get; }
    IDbSet<MissionGeography> MissionGeography { get; }
    IDbSet<Member> Members { get; }
    IDbSet<PersonAddress> PersonAddress { get; }
    IDbSet<PersonContact> PersonContact { get; }
    IDbSet<MemberUnitDocument> MemberUnitDocuments { get; }
    IDbSet<Subject> Subjects { get; }
    IDbSet<SubjectGroup> SubjectGroups { get; }
    IDbSet<SubjectGroupLink> SubjectGroupLinks { get; }
    IDbSet<Training> Trainings { get; }
    IDbSet<TrainingAward> TrainingAward { get; }
    IDbSet<TrainingCourse> TrainingCourses { get; }
    IDbSet<Document> Documents { get; }
    IDbSet<TrainingRoster> TrainingRosters { get; }
    IDbSet<TrainingRule> TrainingRules { get; }
    IDbSet<SarUnit> Units { get; }
    IDbSet<UnitApplicant> UnitApplicants { get; }
    IDbSet<UnitMembership> UnitMemberships { get; }
    IDbSet<UnitStatus> UnitStatusTypes { get; }
    IDbSet<UnitDocument> UnitDocuments { get; }
    IDbSet<ComputedTrainingAward> ComputedTrainingAwards { get; }
    IDbSet<TrainingExpirationSummary> TrainingExpirationSummaries { get; }
    IDbSet<CurrentMemberIds> CurrentMemberIds { get; }
    IDbSet<xref_county_id> xref_county_id { get; }

    int SaveChanges();
    void RecordSensitiveAccess(SensitiveInfoAccess record);
  }
}