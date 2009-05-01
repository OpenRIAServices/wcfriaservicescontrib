
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
    using System.Data.Objects.DataClasses;
    using System.Data;


    // The MetadataTypeAttribute identifies PersonMetadata as the class
    // that carries additional metadata for the Person class.
    [MetadataTypeAttribute(typeof(Person.PersonMetadata))]
    public partial class Person
    {

#pragma warning disable 649    // temporarily disable compiler warnings about unassigned fields

        // This class allows you to attach custom attributes to properties
        // of the Person class.
        //
        // For example, the following marks the Xyz property as a
        // required field and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression(“[A-Z][A-Za-z0-9]*”)]
        //    [StringLength(32)]
        //    public string Xyz;
        internal sealed class PersonMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private PersonMetadata()
            {
            }

            public int PersonID;

            public string LastName;

            public string FirstName;

            public EntityCollection<CourseGrade> CourseGrade;

            public OfficeAssignment OfficeAssignment;

            public EntityCollection<Course> Course;

            public EntityState EntityState;
        }

#pragma warning restore 649    // re-enable compiler warnings about unassigned fields
    }

    // The MetadataTypeAttribute identifies CourseMetadata as the class
    // that carries additional metadata for the Course class.
    [MetadataTypeAttribute(typeof(Course.CourseMetadata))]
    public partial class Course
    {

#pragma warning disable 649    // temporarily disable compiler warnings about unassigned fields

        // This class allows you to attach custom attributes to properties
        // of the Course class.
        //
        // For example, the following marks the Xyz property as a
        // required field and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression(“[A-Z][A-Za-z0-9]*”)]
        //    [StringLength(32)]
        //    public string Xyz;
        internal sealed class CourseMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private CourseMetadata()
            {
            }

            public int CourseID;

            public string Title;

            public int Credits;

            public Department Department;

            public EntityCollection<CourseGrade> CourseGrade;
            
            public EntityCollection<Person> Person;

            public EntityState EntityState;
        }

#pragma warning restore 649    // re-enable compiler warnings about unassigned fields
    }
    // The MetadataTypeAttribute identifies CourseMetadata as the class
    // that carries additional metadata for the Course class.
    [MetadataTypeAttribute(typeof(OnlineCourse.CourseMetadata))]
    public partial class OnlineCourse
    {

#pragma warning disable 649    // temporarily disable compiler warnings about unassigned fields

        // This class allows you to attach custom attributes to properties
        // of the Course class.
        //
        // For example, the following marks the Xyz property as a
        // required field and specifies the format for valid values:
        //    [Required]
        //    [RegularExpression(“[A-Z][A-Za-z0-9]*”)]
        //    [StringLength(32)]
        //    public string Xyz;
        internal sealed class OnlineCourseMetadata
        {

            // Metadata classes are not meant to be instantiated.
            private OnlineCourseMetadata()
            {
            }

            public int CourseID;

            public string Title;

            public int Credits;

            public Department Department;

            public EntityCollection<CourseGrade> CourseGrade;
            
            public EntityCollection<Person> Person;

            public EntityState EntityState;
        }

#pragma warning restore 649    // re-enable compiler warnings about unassigned fields
    }
}
