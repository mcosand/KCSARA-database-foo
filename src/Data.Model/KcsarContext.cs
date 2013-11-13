namespace Kcsar.Database.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public partial class KcsarContext : DbContext, IKcsarContext
    {
        public IDbSet<Animal> Animals { get; set; }
        public IDbSet<AnimalMission> AnimalMissions { get; set; }
        public IDbSet<AnimalOwner> AnimalOwners { get; set; }
        public IDbSet<Mission> Missions { get; set; }
        public IDbSet<MissionDetails> MissionDetails { get; set; }
        public IDbSet<MissionLog> MissionLog { get; set; }
        public IDbSet<MissionRoster> MissionRosters { get; set; }
        public IDbSet<MissionGeography> MissionGeography { get; set; }
        public IDbSet<Member> Members { get; set; }
        public IDbSet<PersonAddress> PersonAddress { get; set; }
        public IDbSet<PersonContact> PersonContact { get; set; }
        public IDbSet<MemberUnitDocument> MemberUnitDocuments { get; set; }
        public IDbSet<Subject> Subjects { get; set; }
        public IDbSet<SubjectGroup> SubjectGroups { get; set; }
        public IDbSet<SubjectGroupLink> SubjectGroupLinks { get; set; }
        public IDbSet<Training> Trainings { get; set; }
        public IDbSet<TrainingAward> TrainingAward { get; set; }
        public IDbSet<TrainingCourse> TrainingCourses { get; set; }
        public IDbSet<Document> Documents { get; set; }
        public IDbSet<TrainingRoster> TrainingRosters { get; set; }
        public IDbSet<TrainingRule> TrainingRules { get; set; }
        public IDbSet<SarUnit> Units { get; set; }
        public IDbSet<UnitApplicant> UnitApplicants { get; set; }
        public IDbSet<UnitMembership> UnitMemberships { get; set; }
        public IDbSet<UnitStatus> UnitStatusTypes { get; set; }
        public IDbSet<UnitDocument> UnitDocuments { get; set; }
        public IDbSet<ComputedTrainingAward> ComputedTrainingAwards { get; set; }
        public IDbSet<TrainingExpirationSummary> TrainingExpirationSummaries { get; set; }
        public IDbSet<CurrentMemberIds> CurrentMemberIds { get; set; }
        public IDbSet<xref_county_id> xref_county_id { get; set; }
        public IDbSet<SensitiveInfoAccess> SensitiveInfoLog { get; set; }
        protected IDbSet<AuditLog> AuditLog { get; set; }

        public KcsarContext()
            : base()
        {
            this.AuditLog = this.Set<AuditLog>();
        }

        public KcsarContext(string connectionStringOrName)
            : base(connectionStringOrName)
        {
            this.AuditLog = this.Set<AuditLog>();
        }


        public static readonly DateTime MinEntryDate = new DateTime(1945, 1, 1);

        private Dictionary<Type, List<PropertyInfo>> reportingProperties = new Dictionary<Type, List<PropertyInfo>>();
        private Dictionary<string, string> reportingFormats = new Dictionary<string, string>();
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Mission>().HasOptional(f => f.Details).WithRequired(f => f.Mission);
            modelBuilder.Entity<Member>().HasOptional(f => f.MedicalInfo).WithRequired(f => f.Member);
        }

        public Func<UnitMembership, bool> GetActiveMembershipFilter(Guid? unit, DateTime time)
        {
            // Keep this expression in sync with the one in GetActiveMembers
            return
                g => (!unit.HasValue || g.Unit.Id == unit) && g.Status.IsActive && g.Activated <= time && (g.EndTime == null || g.EndTime > time);
        }

        // Gets members that are active with at least one unit at a specific time, sorted by lastname,firstname
        public IQueryable<Member> GetActiveMembers(Guid? unit, DateTime time, params string[] includes)
        {
            IQueryable<Member> source = this.Members.Include(includes);

            var active = source.OrderBy(f => f.LastName).ThenBy(f => f.FirstName).Where(
                f => f.Memberships.Any(
                    // Keep this in sync with the expression in GetActiveMembershipFilter - EntityFramework has problems using the expression
                    //  in nested delegates
                    g => (!unit.HasValue || g.Unit.Id == unit) && g.Status.IsActive && g.Activated <= time && (g.EndTime == null || g.EndTime > time)
                ));

            return active;
        }

        public AuditLog[] GetLog(DateTime since)
        {
            var log = this.AuditLog.AsNoTracking().Where(f => f.Changed >= since).OrderByDescending(f => f.Changed).AsEnumerable();
            return log.Select(f => f.GetCopy()).ToArray();
        }
    }
}
