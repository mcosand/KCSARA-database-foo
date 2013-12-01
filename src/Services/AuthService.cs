/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Services
{
  using System;
  using System.Linq;
  using System.Threading;
  using System.Web.Profile;
  using System.Web.Security;
  using Kcsar.Membership;

  public class AuthService : IPermissionsService
  {
    public Guid UserId
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSelf(Guid id)
    {
      throw new NotImplementedException();
    }

    public bool IsAdmin
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsAuthenticated
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsUser
    {
      get { throw new NotImplementedException(); }
    }

    public bool InGroup(params string[] group)
    {
      throw new NotImplementedException();
    }

    public bool IsMembershipForPerson(Guid id)
    {
      throw new NotImplementedException();
    }

    public bool IsMembershipForUnit(Guid id)
    {
      throw new NotImplementedException();
    }

    public bool IsUserOrLocal(System.Web.HttpRequestBase request)
    {
      throw new NotImplementedException();
    }

    public bool IsRoleForPerson(string role, Guid personId)
    {
      throw new NotImplementedException();
    }

    public bool IsRoleForUnit(string role, Guid unitId)
    {
      throw new NotImplementedException();
    }

    public void SetCurrentUser(string username)
    {
      FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(username, false, 5);
      Thread.CurrentPrincipal = new RolePrincipal(new FormsIdentity(ticket));
    }

    public void AddUserToRole(string username, string roleName)
    {
      throw new NotImplementedException();
    }

    public void CreateRole(string roleName)
    {
      throw new NotImplementedException();
    }

    public bool RoleExists(string roleName)
    {
      throw new NotImplementedException();
    }

    public System.Web.Security.MembershipUser GetUser(string username)
    {
      throw new NotImplementedException();
    }

    public System.Web.Security.MembershipUser CreateUser(string username, string password, string email)
    {
      throw new NotImplementedException();
    }

    public void UpdateUser(System.Web.Security.MembershipUser user)
    {
      throw new NotImplementedException();
    }

    public bool DeleteUser(string username)
    {
      throw new NotImplementedException();
    }


    public KcsarUserProfile GetProfile(string username)
    {
      return ProfileBase.Create(username) as KcsarUserProfile;
    }
  }
}
