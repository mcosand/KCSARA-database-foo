/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Internal.Data.Model.Tests.Entities
{
  using System;
  using System.Collections;
  using Kcsar.Database.Model;
  using NUnit.Framework;

  [TestFixture]
  public abstract class EntityTestFixture<T> where T : ModelObject, new()
  {
    [Test]
    public void CollectionsAreVirtual()
    {
      Type t = typeof(T);
      foreach (var property in t.GetProperties())
      {
        if (!property.PropertyType.IsSubclassOf(typeof(IEnumerable))) continue;

        Assert.IsTrue(property.GetGetMethod().IsVirtual, property.Name);
      }
    }

    [Test]
    public void CollectionsAreInitialized()
    {
      T test = new T();

      Type t = typeof(T);
      foreach (var property in t.GetProperties())
      {
        if (property.PropertyType.Name != "ICollection`1") continue;

        Assert.IsNotNull(property.GetValue(test), property.Name);
      }

    }
  }
}
