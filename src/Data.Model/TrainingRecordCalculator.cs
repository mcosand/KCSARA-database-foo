namespace Kcsar.Database.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;

  public class TrainingRecordCalculator
  {
    readonly IKcsarContext db;

    public TrainingRecordCalculator(IKcsarContext db)
    {
      this.db = db;
    }

    public void Calculate()
    {
      foreach (var member in db.Members)
      {
        Calculate(new[] { member }, DateTime.Now);
      }
    }

    public void Calculate(Guid memberId)
    {
      // Recalculate the effective training awards for a specific member.
      Calculate(from m in db.Members where m.Id == memberId select m);
    }

    public void Calculate(Guid memberId, DateTime time)
    {
      // Recalculate the effective training awards for a specific member.
      Calculate(from m in db.Members where m.Id == memberId select m, time);
    }

    public void Calculate(IEnumerable<Member> members)
    {
      Calculate(members, DateTime.Now);
    }

    public List<ComputedTrainingAward[]> Calculate(IEnumerable<Member> members, DateTime time)
    {
      List<ComputedTrainingAward[]> retVal = new List<ComputedTrainingAward[]>();

      // TODO: only use the rules in effect at time 'time'
      List<TrainingRule> rules = (from r in db.TrainingRules select r).ToList();

      Dictionary<Guid, TrainingCourse> courses = (from c in db.TrainingCourses select c).ToDictionary(x => x.Id);

      foreach (Member m in members)
      {
        foreach (ComputedTrainingAward award in (from a in db.ComputedTrainingAwards where a.Member.Id == m.Id select a))
        {
          db.ComputedTrainingAwards.Remove(award);
        }

        // Sort by expiry and completed dates to handle the case of re-taking a course that doesn't expire.
        var direct = (from a in db.TrainingAward.Include("Course") where a.Member.Id == m.Id && a.Completed <= time select a)
            .OrderBy(f => f.Course.Id).ThenByDescending(f => f.Expiry).ThenByDescending(f => f.Completed);

        Dictionary<Guid, ComputedTrainingAward> awards = new Dictionary<Guid, ComputedTrainingAward>();

        Guid lastCourse = Guid.Empty;
        foreach (TrainingAward a in direct)
        {
          //if (this.Entry(a).State == EntityState.Deleted)
          //{
          //  continue;
          //}

          if (a.Course.Id != lastCourse)
          {
            var ca = new ComputedTrainingAward(a);
            awards.Add(a.Course.Id, ca);
            db.ComputedTrainingAwards.Add(ca);
            lastCourse = a.Course.Id;
          }
        }

        bool awardInLoop = false;
        do
        {
          awardInLoop = false;

          foreach (TrainingRule rule in rules)
          {
            //  source>result>prerequisite
            string[] fields = rule.RuleText.Split('>');

            if (fields.Length > 2)
            {
              var prereqs = fields[2].Split('+');
              // Keep going only if /all/ of the prereqs are met by /any/ of the existing awards, 
              if (!prereqs.All(f => awards.Keys.Any(g => g.ToString().Equals(f, StringComparison.OrdinalIgnoreCase))))
              {
                continue;
              }
            }

            if (fields[0].StartsWith("Mission"))
            {
              //Mission(12:%:36)
              Match match = Regex.Match(fields[0], @"Mission\((\d+):([^:]+):(\d+)\)", RegexOptions.IgnoreCase);
              if (match.Success == false)
              {
                throw new InvalidOperationException("Can't understand rule: " + fields[0]);
              }

              int requiredHours = int.Parse(match.Groups[1].Value);
              string missionType = match.Groups[2].Value;
              int monthSpan = int.Parse(match.Groups[3].Value);

              var missions = (from r in db.MissionRosters where r.Person.Id == m.Id && r.TimeIn < time select r);
              if (missionType != "%")
              {
                missions = missions.Where(x => x.Mission.MissionType.Contains(missionType));
              }
              missions = missions.OrderByDescending(x => x.TimeIn);

              double sum = 0;
              DateTime startDate = DateTime.Now;
              foreach (MissionRoster roster in missions)
              {
                if (roster.TimeIn.HasValue && (roster.InternalRole != MissionRoster.ROLE_IN_TOWN && roster.InternalRole != MissionRoster.ROLE_NO_ROLE))
                {
                  startDate = roster.TimeIn.Value;
                  sum += roster.Hours ?? 0.0;

                  if (sum > requiredHours)
                  {
                    awardInLoop |= RewardTraining(m, courses, awards, rule, startDate, startDate.AddMonths(monthSpan), fields[1]);
                    break;
                  }
                }
              }
            }
            else
            {
              Guid?[] sources = fields[0].Split('+').Select(f => f.ToGuid()).ToArray();

              if (sources.Any(f => f == null))
              {
                throw new InvalidOperationException("Unknown rule type: " + rule.Id);
              }

              if (sources.All(f => awards.ContainsKey(f.Value)))
              {
                DateTime? completed = sources.Max(f => awards[f.Value].Completed);
                DateTime? expiry = null;
                if (sources.Any(f => awards[f.Value].Expiry != null))
                {
                  expiry = sources.Min(f => awards[f.Value].Expiry ?? DateTime.MaxValue);
                }
                awardInLoop |= RewardTraining(m, courses, awards, rule, completed, expiry, fields[1]);
              }
            }
          }
        } while (awardInLoop);
        retVal.Add(awards.Values.ToArray());
      }
      return retVal;
    }

    protected bool RewardTraining(Member m, Dictionary<Guid, TrainingCourse> courses, Dictionary<Guid, ComputedTrainingAward> awards, TrainingRule rule, DateTime? completed, DateTime? expiry, string newAwardsString)
    {
      IEnumerable<string> results = newAwardsString.Split('+');
      bool awarded = false;

      foreach (string result in results)
      {
        string[] parts = result.Split(':');
        Guid course = new Guid(parts[0]);

        if (!courses.ContainsKey(course))
        {
          throw new InvalidOperationException("Found bad rule: Adds course with ID" + course.ToString());
        }

        if (parts.Length > 1)
        {
          if (parts[1] == "default")
          {
            if (courses[course].ValidMonths.HasValue)
            {
              expiry = completed.Value.AddMonths(courses[course].ValidMonths.Value);
            }
            else
            {
              expiry = null;
            }
          }
          else
          {
            expiry = completed.Value.AddMonths(int.Parse(parts[1]));
          }
        }


        if (awards.ContainsKey(course) && expiry > awards[course].Expiry)
        {
          awards[course].Completed = completed;
          awards[course].Expiry = expiry;
          awards[course].Rule = rule;
          awarded = true;
        }
        else if (!awards.ContainsKey(course))
        {
          ComputedTrainingAward newAward = new ComputedTrainingAward { Course = courses[course], Member = m, Completed = completed, Expiry = expiry, Rule = rule };
          awards.Add(course, newAward);
          db.ComputedTrainingAwards.Add(newAward);
          awarded = true;
        }
      }
      return awarded;
    }
  }
}
