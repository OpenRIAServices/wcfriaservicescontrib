

using System;
namespace RiaClassManagerCS.Web
{
    #region "Course"
    public interface ICourse
    {
        int CourseID { get; set; }
        String Title { get; set; }
        int Credits { get; set; }
    }
    #endregion

    #region "CourseGrade"
    public interface ICourseGrade
    {
        int EnrollmentID { get; set; }
        decimal? Grade { get; set; }
    }
    #endregion

    #region "Department"
    public interface IDepartment
    {
        int DepartmentID { get; set; }
        String Name { get; set; }
        decimal Budget { get; set; }
        DateTime StartDate { get; set; }
        int? Administrator { get; set; }
    }
    #endregion

    #region "OfficeAssignment"
    public interface IOfficeAssignment
    {
        int InstructorID { get; set; }
        String Location { get; set; }
    }
    #endregion

    #region "Person"
    public interface IPerson
    {
        int PersonID { get; set; }
        String LastName { get; set; }
        String FirstName { get; set; }
    }
    #endregion

    #region "OnlineCourse"
    public interface IOnlineCourse : ICourse
    {
        String URL { get; set; }
    }
    #endregion

    #region "OnsiteCourse"
    public interface IOnsiteCourse : ICourse
    {
        String Location { get; set; }
        String Days { get; set; }
        DateTime Time { get; set; }
    }
    #endregion

    #region "Student"
    public interface IStudent : IPerson
    {
        DateTime EnrollmentDate { get; set; }
    }
    #endregion

    #region "Teacher"
    public interface ITeacher : IPerson
    {
        DateTime HireDate { get; set; }
    }
    public partial class Teacher : ITeacher
    {

    }
    #endregion

}














