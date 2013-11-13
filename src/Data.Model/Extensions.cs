using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Infrastructure;

namespace System.Linq
{
    public static class Extensions
    {
        public static Guid GetId(this EntityReference obj)
        {
            return (Guid)obj.EntityKey.EntityKeyValues.First().Value;
        }

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
