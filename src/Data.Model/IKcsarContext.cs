namespace Kcsar.Database.Model
{
    using System.Data.Entity;

    public interface IKcsarContext
    {
        IDbSet<Animal> Animals { get; set; }
        IDbSet<AnimalMission> AnimalMissions { get; set; }
        IDbSet<AnimalOwner> AnimalOwners { get; set; }
        IDbSet<Mission> Missions { get; set; }
        IDbSet<MissionDetails> MissionDetails { get; set; }
        IDbSet<MissionLog> MissionLog { get; set; }
        IDbSet<MissionRoster> MissionRosters { get; set; }
        IDbSet<MissionGeography> MissionGeography { get; set; }
        IDbSet<Member> Members { get; set; }
        IDbSet<PersonAddress> PersonAddress { get; set; }
        IDbSet<PersonContact> PersonContact { get; set; }
        IDbSet<MemberUnitDocument> MemberUnitDocuments { get; set; }
        IDbSet<Subject> Subjects { get; set; }
        IDbSet<SubjectGroup> SubjectGroups { get; set; }
        IDbSet<SubjectGroupLink> SubjectGroupLinks { get; set; }
        IDbSet<Training> Trainings { get; set; }
        IDbSet<TrainingAward> TrainingAward { get; set; }
        IDbSet<TrainingCourse> TrainingCourses { get; set; }
        IDbSet<Document> Documents { get; set; }
        IDbSet<TrainingRoster> TrainingRosters { get; set; }
        IDbSet<TrainingRule> TrainingRules { get; set; }
        IDbSet<SarUnit> Units { get; set; }
        IDbSet<UnitApplicant> UnitApplicants { get; set; }
        IDbSet<UnitMembership> UnitMemberships { get; set; }
        IDbSet<UnitStatus> UnitStatusTypes { get; set; }
        IDbSet<UnitDocument> UnitDocuments { get; set; }
        IDbSet<ComputedTrainingAward> ComputedTrainingAwards { get; set; }
        IDbSet<TrainingExpirationSummary> TrainingExpirationSummaries { get; set; }
        IDbSet<CurrentMemberIds> CurrentMemberIds { get; set; }
        IDbSet<xref_county_id> xref_county_id { get; set; }

        int SaveChanges();
    }
}