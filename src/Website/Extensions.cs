
namespace Kcsar.Database.Web
{
  using System;

  public static class Extensions
  {
    static DateTime ZeroTime = new DateTime(1, 1, 1);
    public static int? Years(this TimeSpan? span)
    {
      if (span == null) return null;
      // Because "Zero" time is rooted in year one, we subject 1 here.
      return (Extensions.ZeroTime + span.Value).Year - 1;
    }
  }
}