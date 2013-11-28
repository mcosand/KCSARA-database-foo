namespace Kcsar.Database.Services
{
  using System;
  using System.Web;
  using System.Web.Security;

  public interface IPermissionsService
  {
    Guid UserId { get; }
    bool IsSelf(Guid id);
    bool IsAdmin { get; }
    bool IsAuthenticated { get; }
    bool IsUser { get; }
    bool InGroup(params string[] group);
    bool IsMembershipForPerson(Guid id);
    bool IsMembershipForUnit(Guid id);
    bool IsUserOrLocal(HttpRequestBase request);
    bool IsRoleForPerson(string role, Guid personId);
    bool IsRoleForUnit(string role, Guid unitId);

    MembershipUser GetUser(string username);
  }
}
