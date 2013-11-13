namespace Kcsar.Database.Model
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public partial class KcsarContext
    {
        private AuditLog GetAuditLogEntry(DbEntityEntry entry)
        {
            string report = string.Empty;
            IModelObject obj = (IModelObject)entry.Entity;

            ObjectStateEntry osEntry = null;
            ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entry.Entity, out osEntry);

            if (entry.State == EntityState.Added)
            {
                report = obj.GetReportHtml();
            }
            else if (entry.State == EntityState.Modified)
            {
                var original = entry.OriginalValues.Clone().ToObject();
                foreach (var property in GetReportableProperties(entry))
                {
                    var left = property.GetValue(original, null);
                    var right = property.GetValue(entry.Entity, null);

                    // If it is not true that the left and right values are the same...
                    if (!((left == null && right == null) || (left != null && left.Equals(right))))
                    {
                        report += string.Format("{0}: {1} => {2}<br/>", property.Name, left, right);
                    }
                }
            }
            else if (entry.State == EntityState.Deleted)
            {
                var original = (IModelObject)entry.OriginalValues.Clone().ToObject();
                report = original.GetReportHtml();
            }

            return new AuditLog
            {
                Action = entry.State.ToString(),
                Comment = report,
                Collection = osEntry.EntitySet.Name,
                Changed = DateTime.Now,
                ObjectId = ((IModelObject)entry.Entity).Id,
                User = Thread.CurrentPrincipal.Identity.Name
            };
        }

        public override int SaveChanges()
        {
            ObjectContext oc = ((IObjectContextAdapter)this).ObjectContext;
            var osm = oc.ObjectStateManager;

            oc.DetectChanges();

            List<AuditLog> changes = new List<AuditLog>();

            foreach (var entry in this.ChangeTracker.Entries().Where(f => (f.State != EntityState.Unchanged && f.State != EntityState.Detached) && f.Entity is IModelObject))
            {
                changes.Add(GetAuditLogEntry(entry));
            }

            //List<RuleViolation> errors = new List<RuleViolation>();

            //KcsarContext comparisonContext = new KcsarContext(this.Database.Connection.ConnectionString);

            //List<AuditLog> changes = new List<AuditLog>();

            //// Validate the state of each entity in the context
            //// before SaveChanges can succeed.
            //Random rand = new Random();

            //ObjectContext oc = ((IObjectContextAdapter)this).ObjectContext;
            //var osm = oc.ObjectStateManager;

            //oc.DetectChanges();

            //// Added and modified objects - we can describe the state of the object with
            //// the information already present.
            //foreach (ObjectStateEntry entry in
            //    osm.GetObjectStateEntries(
            //    EntityState.Added | EntityState.Modified))
            //{
            //    // Do Validation
            //    if (entry.Entity is IValidatedEntity)
            //    {
            //        IValidatedEntity validate = (IValidatedEntity)entry.Entity;
            //        if (!validate.Validate())
            //        {
            //            errors.AddRange(validate.Errors);
            //        }
            //        else
            //        {
            //            Document d = entry.Entity as Document;
            //            if (d != null)
            //            {
            //                if (string.IsNullOrWhiteSpace(d.StorePath))
            //                {
            //                    string path = string.Empty;
            //                    for (int i = 0; i < Document.StorageTreeDepth; i++)
            //                    {
            //                        path += ((i > 0) ? "\\" : "") + rand.Next(Document.StorageTreeSpan).ToString();
            //                    }
            //                    if (!System.IO.Directory.Exists(Document.StorageRoot + path))
            //                    {
            //                        System.IO.Directory.CreateDirectory(Document.StorageRoot + path);
            //                    }
            //                    path += "\\" + d.Id.ToString();
            //                    d.StorePath = path;
            //                }
            //                System.IO.File.WriteAllBytes(Document.StorageRoot + d.StorePath, d.Contents);
            //            }
            //            // New values are valid

            //            if (entry.Entity is IModelObject)
            //            {
            //                IModelObject obj = (IModelObject)entry.Entity;

            //                // Keep track of the change for reporting.
            //                obj.LastChanged = DateTime.Now;
            //                obj.ChangedBy = Thread.CurrentPrincipal.Identity.Name;

            //                IModelObject original = (entry.State == EntityState.Added) ? null : GetOriginalVersion(comparisonContext, entry);


            //                if (original == null)
            //                {
            //                    changes.Add(new AuditLog
            //                    {
            //                        Action = entry.State.ToString(),
            //                        Comment = obj.GetReportHtml(),
            //                        Collection = entry.EntitySet.Name,
            //                        Changed = DateTime.Now,
            //                        ObjectId = obj.Id,
            //                        User = Thread.CurrentPrincipal.Identity.Name
            //                    });
            //                }
            //                else
            //                {
            //                    string report = string.Format("<b>{0}</b><br/>", obj);

            //                    foreach (PropertyInfo pi in GetReportableProperties(entry.EntitySet.ElementType))
            //                    {
            //                        object left = pi.GetValue(original, null);
            //                        object right = pi.GetValue(obj, null);
            //                        if ((left == null && right == null) || (left != null && left.Equals(right)))
            //                        {
            //                            //   report += string.Format("{0}: unchanged<br/>", pi.Name);
            //                        }
            //                        else
            //                        {
            //                            report += string.Format("{0}: {1} => {2}<br/>", pi.Name, left, right);
            //                        }
            //                    }
            //                    changes.Add(new AuditLog
            //                    {
            //                        Action = entry.State.ToString(),
            //                        Comment = report,
            //                        Collection = entry.EntitySet.Name,
            //                        Changed = DateTime.Now,
            //                        ObjectId = obj.Id,
            //                        User = Thread.CurrentPrincipal.Identity.Name
            //                    });
            //                }
            //            }
            //        }
            //    }
            //}

            //// Added and modified objects - we need to fetch more data before we can report what the change was in readable form.
            //foreach (ObjectStateEntry entry in osm.GetObjectStateEntries(EntityState.Deleted))
            //{
            //    IModelObject modelObject = GetOriginalVersion(comparisonContext, entry);
            //    if (modelObject != null)
            //    {
            //        Document d = modelObject as Document;
            //        if (d != null && !string.IsNullOrWhiteSpace(d.StorePath))
            //        {
            //            string path = Document.StorageRoot + d.StorePath;
            //            System.IO.File.Delete(path);
            //            for (int i = 0; i < Document.StorageTreeDepth; i++)
            //            {
            //                path = System.IO.Path.GetDirectoryName(path);
            //                if (System.IO.Directory.GetDirectories(path).Length + System.IO.Directory.GetFiles(path).Length == 0)
            //                {
            //                    System.IO.Directory.Delete(path);
            //                }
            //            }
            //        }
            //        changes.Add(new AuditLog
            //        {
            //            Action = entry.State.ToString(),
            //            Comment = modelObject.GetReportHtml(),
            //            Collection = entry.EntitySet.Name,
            //            Changed = DateTime.Now,
            //            ObjectId = modelObject.Id,
            //            User = Thread.CurrentPrincipal.Identity.Name
            //        });
            //    }
            //}

            //if (errors.Count > 0)
            //{
            //    throw new RuleViolationsException(errors);
            //}

            changes.ForEach(f => this.AuditLog.Add(f));

            return base.SaveChanges();
            //if (SavedChanges != null)
            //{
            //    SavedChanges(this, new SavingChangesArgs { Changes = changes.Select(f => f.Comment).ToList() });
            //}
        }

        private IEnumerable<PropertyInfo> GetReportableProperties(DbEntityEntry entry)
        {
            Type forType = entry.Entity.GetType();
            if (!this.reportingProperties.ContainsKey(forType))
            {
                var properties = forType.GetProperties().ToDictionary(f => f.Name, f => f);
                List<string> mappedProperties = entry.CurrentValues.PropertyNames.ToList();

                foreach (string prop in properties.Keys.ToArray())
                {
                    if (!properties.ContainsKey(prop)) continue;

                    object[] reporting = properties[prop].GetCustomAttributes(typeof(ReportingAttribute), false);
                    if (prop == "LastChanged" || prop == "ChangedBy" || prop == "Id")
                    {
                        properties.Remove(prop);
                    }
                    else if (reporting.Length == 1)
                    {
                        // Keep this property in the list

                        // Hide the ones it hides...
                        ReportingAttribute attrib = (ReportingAttribute)reporting[0];
                        this.reportingFormats.Add(forType.FullName + ':' + prop, attrib.Format);
                        foreach (string pname in attrib.Hides.Split(','))
                        {
                            properties.Remove(pname);
                        }
                    }
                    else if (!mappedProperties.Contains(prop))
                    {
                        properties.Remove(prop);
                    }
                }
                this.reportingProperties.Add(forType, properties.Values.ToList());

                foreach (MemberReportingAttribute attrib in forType.GetCustomAttributes(typeof(MemberReportingAttribute), true))
                {
                    this.reportingFormats.Add(forType.FullName + ':' + attrib.Property, attrib.Format);
                }
            }
            return this.reportingProperties[forType];
        }
    }
}
