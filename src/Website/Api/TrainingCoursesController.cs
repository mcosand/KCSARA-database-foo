/*
 * Copyright (c) 2013 Matt Cosand
 */
namespace Kcsar.Database.Web.Api
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Web.Http;
  using Kcsar.Database.Services;
  using Kcsar.Database.Web.Api.Models;
  using log4net;
  using Data = Kcsar.Database.Model;

  public class TrainingCoursesController : DatabaseApiController
  {
    public TrainingCoursesController(
      IPermissionsService permissions,
      Data.IKcsarContext db,
      IWebHostingService hosting,
      ILog log)
      : base(db, permissions, hosting, log)
    {
    }

    [HttpGet]
    public IEnumerable<TrainingCourse> Get()
    {
      DateTime now = DateTime.Now;
      return GetCourseViews(f => (f.OfferedFrom ?? DateTime.MaxValue) > now, now);
    }

    [HttpGet]
    public IEnumerable<TrainingCourse> GetAll()
    {
      DateTime now = DateTime.Now;
      return GetCourseViews(f => true, now);
    }

    private IEnumerable<TrainingCourse> GetCourseViews(Expression<Func<Data.TrainingCourse, bool>> whereClause, DateTime when)
    {
      return db.TrainingCourses
         .Where(whereClause)
         .OrderBy(f => f.DisplayName)
         .Select(TrainingCourse.GetDataModelConversion(when));
    }


    // GET api/<controller>/5
    [HttpGet]
    public TrainingCourse Get(Guid id)
    {
      // Get the data object (if it exists), and pass it through a conversion to the view model. If it doesn't exist, throw a 404 exception
      return GetObjectOrNotFound(
          () => db.TrainingCourses
          .Where(f => f.Id == id)
          .Select(TrainingCourse.GetDataModelConversion(DateTime.Now))
          .SingleOrDefault());
    }

    // POST api/<controller>
    public void Post([FromBody]string value)
    {
    }

    // PUT api/<controller>/5
    public void Put(int id, [FromBody]string value)
    {
    }

    // DELETE api/<controller>/5
    public void Delete(int id)
    {
    }
  }
}