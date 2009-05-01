
namespace RiaClassManagerCS.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Ria;
    using System.Web.Ria.Data;
    using System.Web.DomainServices;
    using System.Data;
    using System.Web.DomainServices.LinqToEntities;


    // Implements application logic using the SchoolEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    [EnableClientAccess()]
    public class SchoolService : LinqToEntitiesDomainService<SchoolEntities>
    {

        // TODO: Consider
        // 1. Adding parameters to this method and constraining returned results, and/or
        // 2. Adding query methods taking different parameters.
        public IQueryable<Teacher> GetTeachers()
        {
            return (IQueryable<Teacher>)from tr in this.Context.Persons where tr is Teacher select tr;
        }
        //public IQueryable<OnlineCourse> GetOnlineCources()
        //{
        //    return (IQueryable<OnlineCourse>)from oc in this.Context.Courses where oc is OnlineCourse select oc;
        //}

        public void InsertTeacher(Teacher Teacher)
        {
            this.Context.AddToPersons(Teacher);
        }

        public void UpdateTeacher(Teacher currentTeacher, Teacher originalTeacher)
        {
            this.Context.AttachAsModified(currentTeacher, originalTeacher);
        }

        public void DeleteTeacher(Teacher Teacher)
        {
            if ((Teacher.EntityState == EntityState.Detached))
            {
                this.Context.Attach(Teacher);
            }
            this.Context.DeleteObject(Teacher);
        }
    }
}


