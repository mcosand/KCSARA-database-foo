namespace System.Linq
{
  using System.Data.Entity.Infrastructure;

  public static class Extensions
  {
    public static IQueryable<T> Include<T>(this IQueryable<T> query, params string[] includes)
    {
      DbQuery<T> dbQuery = query as DbQuery<T>;
      if (dbQuery == null) return query;

      foreach (var include in includes)
      {
        dbQuery = dbQuery.Include(include);
      }
      return dbQuery;
    }
  }
}
