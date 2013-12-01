﻿/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Membership
{
  public interface INestedRoleProvider
  {
    ExtendedRole ExtendedGetRole(string roleName);
    void UpdateRole(ExtendedRole role, string originalName);

    void AddRoleToRole(string child, string parent);
    void RemoveRoleFromRole(string child, string parent);

    string[] GetRolesInRole(string roleName, bool recurse);

    string[] GetUsersInRole(string roleName, bool recurse);
    string[] GetRolesForUser(string username, bool recurse);
    bool IsUserInRole(string username, string roleName, bool recurse);
    string[] FindUsersInRole(string roleName, string usernameToMatch, bool recurse);
  }
}
