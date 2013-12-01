/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Internal.Data.Model.Tests
{
  using System;
  using System.Data.Entity;
  using System.Data.Entity.Infrastructure;
  using System.Data.Entity.Migrations;

  public class MigrateDbToLatestInitializerConnString<TContext, TMigrationsConfiguration> : IDatabaseInitializer<TContext>
    where TContext : DbContext
    where TMigrationsConfiguration : DbMigrationsConfiguration<TContext>, new()
  {
    public void InitializeDatabase(TContext context)
    {
      if (context == null)
      {
        throw new ArgumentException("Context passed to InitializeDatabase can not be null");
      }

      var config = new TMigrationsConfiguration
      {
        TargetDatabase = new DbConnectionInfo(context.Database.Connection.ConnectionString, "System.Data.SqlClient")
      };

      var migrator = new DbMigrator(config);

      migrator.Update();
    }
  }
}
