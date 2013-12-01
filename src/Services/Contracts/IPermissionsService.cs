/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Services
{
  using System;
  using System.Web;
  using System.Web.Profile;
  using System.Web.Security;
  using Kcsar.Membership;

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

    void SetCurrentUser(string username);

    void AddUserToRole(string username, string roleName);
    void CreateRole(string roleName);
    bool RoleExists(string roleName);

    MembershipUser GetUser(string username);
    MembershipUser CreateUser(string username, string password, string email);
    void UpdateUser(MembershipUser user);
    bool DeleteUser(string username);

    // This doesn't really belong here, but here it stays until I find a better place to put it.
    KcsarUserProfile GetProfile(string username);
  }
}
