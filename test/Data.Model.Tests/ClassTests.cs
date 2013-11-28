using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kcsar.Database.Model;
using NUnit.Framework;

namespace Internal.Data.Model.Tests
{
    [TestFixture]
    public class ClassTests
    {
        private static Type[] IgnoredTypes = new[] { typeof(ReportingAttribute), typeof(MemberReportingAttribute) };

        [Test]
        public void EntitiesHaveTestClass()
        {
            Assembly codefirstAssembly = typeof(KcsarContext).Assembly;

            List<Type> entityTypes = codefirstAssembly.GetExportedTypes().Where(t => typeof(IModelObject).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).ToList();

            foreach (Type testType in this.GetType().Assembly.GetExportedTypes())
            {
                if (testType.BaseType.Name == "EntityTestFixture`1")
                {
                    entityTypes.Remove(testType.BaseType.GetGenericArguments().First());
                }
            }

            Console.WriteLine(string.Join("\n", entityTypes.Select(f => f.ToString())));
            Assert.AreEqual(0, entityTypes.Count, "See stdout");
        }
    }
}
