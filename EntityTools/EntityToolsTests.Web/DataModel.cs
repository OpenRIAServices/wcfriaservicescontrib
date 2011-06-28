using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace EntityToolsTests.Web
{
    public class Person
    {
        [DataMember]
        [Key]
        public Guid Id { get; set; }
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Department
    {
        [Key]
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [Association("FK_Department_DepartmentHead", "DepartmentHeadId", "Id", IsForeignKey = true)]
        public Person DepartmentHead { get; set; }

        [DataMember]
        public Guid DepartmentHeadId { get; set; }

        [Association("FK_Class_Department", "Id", "DepartmentId")]
        public List<Class> Classes { get; set; }
    }

    [DataContract(IsReference = true)]
    public class Class
    {
        [Key]
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [Association("FK_Class_Department", "DepartmentId", "Id", IsForeignKey = true)]
        public Department Department { get; set; }

        [DataMember]
        public Guid DepartmentId { get; set; }
    }
}
