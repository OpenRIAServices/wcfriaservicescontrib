
namespace EntityToolsTests.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class EntityToolsDomainService : DomainService
    {
        private static readonly Guid MATH_DEPT_HEAD_ID = new Guid("{5A6AF025-8FE6-4ED1-A818-4E876ED9BB19}");
        private static readonly Guid PHYSICS_DEPT_HEAD_ID = new Guid("{F2146D09-B936-4FC3-8FDE-5CF112AE0F40}");
        private static readonly Guid ART_DEPT_HEAD_ID = new Guid("{5D07BC71-0F65-4BA5-ABA0-2A0BA93E1733}");

        private static readonly Guid MATH_DEPT_ID = new Guid("{365D784B-1E95-49AE-8879-F607865E1540}");
        private static readonly Guid PHYSICS_DEPT_ID = new Guid("{B214A6E1-A38E-4FC8-B503-115E197EF53D}");
        private static readonly Guid ART_DEPT_ID = new Guid("{220C7225-653C-4CCC-A16B-4AF84CE87B73}");

        private static readonly Guid MATH101_ID = new Guid("{58256A6F-98B0-4187-B838-43D61373B6A3}");
        private static readonly Guid MATH102_ID = new Guid("{D78636B2-94FF-461D-9CA7-F90F18BBABD6}");
        private static readonly Guid MATH103_ID = new Guid("{92FFB034-6F48-4D1A-9211-0BB7D41F6EA2}");
        private static readonly Guid PHYSICS201_ID = new Guid("{611E12DD-74AA-4B4B-8E14-604E73963436}");
        private static readonly Guid PHYSICS205_ID = new Guid("{2456A2AE-2932-43C4-8573-5EBCD32F6878}");
        private static readonly Guid ART101_ID = new Guid("{42D7EADC-7114-450B-A7AE-65D0123A3181}");

        public List<Person> GetPersonSet()
        {
            return new List<Person>
            {
                new Person { Name="John", Id = Guid.NewGuid() },
                new Person { Name = "Andy", Id = Guid.NewGuid() },
                new Person { Name = "Math Dept Head", Id = MATH_DEPT_HEAD_ID },
                new Person { Name = "Physics Guy", Id = PHYSICS_DEPT_HEAD_ID },
                new Person { Name = "Art Gal", Id = ART_DEPT_HEAD_ID }
            };
        }
        public void UpdatePerson(Person person) { }
        public void InsertPerson(Person person) { }

        public List<Department> GetDepartmentSet()
        {
            return new List<Department>
            {
                new Department { Name = "Math", Id = MATH_DEPT_ID, DepartmentHeadId = MATH_DEPT_HEAD_ID },
                new Department { Name = "Physics", Id = PHYSICS_DEPT_ID, DepartmentHeadId = PHYSICS_DEPT_HEAD_ID },
                new Department { Name = "Art", Id = ART_DEPT_ID, DepartmentHeadId = ART_DEPT_HEAD_ID }
            };
        }
        public void UpdateDepartment(Department department) { }
        public void InsertDepartment(Department department) { }

        public List<Class> GetClassSet()
        {
            return new List<Class>
            {
                new Class { Name = "Math 101", Description = "This is the intro math class", DepartmentId = MATH_DEPT_ID, Id = MATH101_ID },
                new Class { Name = "Math 102", Description = "The slightly harder math class", DepartmentId = MATH_DEPT_ID, Id = MATH102_ID },
                new Class { Name = "Math 103", Description = "This one has partial derivatives, so be very afraid", DepartmentId = MATH_DEPT_ID, Id = MATH103_ID },
                new Class { Name = "Physics 201", Description = "Take this on if you don't like engineering", DepartmentId = PHYSICS_DEPT_ID, Id = PHYSICS201_ID },
                new Class { Name = "Physics 205", Description = "This is for engineers, so the curve is really good", DepartmentId = PHYSICS_DEPT_ID, Id = PHYSICS205_ID },
                new Class { Name = "Art 101", Description = "Yes, we're a liberal arts school", DepartmentId = ART_DEPT_ID, Id = ART101_ID }
            };
        }
        public void UpdateClass(Class theClass) { }
        public void InsertClass(Class theClass) { }
        public void DeleteClass(Class theClass) { }

    }
}


