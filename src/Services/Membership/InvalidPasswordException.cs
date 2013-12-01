/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web
{
  using System;

  public class InvalidPasswordException : ApplicationException
  {
    public InvalidPasswordException(string message)
      : base(message)
    {
    }
  }
}