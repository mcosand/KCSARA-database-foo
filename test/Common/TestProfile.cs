/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Internal.Common
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Kcsar.Membership;

  public class TestProfile : KcsarUserProfile
  {
    private string _firstname = null;
    public override string FirstName
    {
      get { return _firstname; }
      set { _firstname = value; }
    }

    private string _lastname = null;
    public override string LastName
    {
      get { return _lastname; }
      set { _lastname = value; }
    }

    private string _key = null;
    public override string LinkKey
    {
      get { return _key; }
      set { _key = value; }
    }

    public override void Save()
    {
      base.Save();
    }
  }
}
