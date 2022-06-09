using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Linq;

namespace Degree_Planner.Database
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement, Column("TermId")]
        public int TermID { get; set; }
        public string TermName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public static List<Term> GetTerms()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                var terms = conn.Table<Term>().ToList();
                return terms;
            }
        
        }
    }
    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement, Column("CourseID")]
        public int CourseID { get; set; }
        public int TermID { get; set; }
        public string CourseName { get; set; }
        public string CourseStatus { get; set; }
        public DateTime CourseStart { get; set; }
        public DateTime CourseEnd { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string CourseNotes { get; set; }
        public bool StartNotification { get; set; }
        public bool EndNotification { get; set; }
        public DateTime startNotificationDate { get; set; }
        public DateTime endNotificationDate { get; set; }
        public static List<Course> GetCourses(int termId)
        {
            int termID = termId;
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                var coursesTemp = from s in conn.Table<Course>()
                                  where s.TermID == termID
                                  select s;
                var courses = coursesTemp.ToList();
                return courses;
            }
        }

    }
    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement, Column("AssessmentID")]
        public int AssessmentID { get; set; }
        public int CourseID { get; set; }
        public string ObjectiveName { get; set; }
        public DateTime ObjectiveNotificationDate { get; set; }
        public DateTime ObjectiveEnd { get; set; }
        public bool ObjectiveNotification { get; set; }
        public string PerformanceName { get; set; }
        public DateTime PerformanceNotificationDate { get; set; }
        public DateTime PerformanceEnd { get; set; }
        public bool PerformanceNotification { get; set; }
        public static Assessment GetAssessment(int courseId)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                Assessment temp = new Assessment();
                temp = conn.Table<Assessment>().FirstOrDefault(x => x.CourseID == courseId);
                return temp;
            }
        }


    }
    [Table ("Notification")]
    public class Notification
    {
        [PrimaryKey, AutoIncrement, Column("NoteID")]
        public int NoteID { get; set; }
        public string NoteMessage { get; set; }
        public DateTime NoteTime { get;set; }

    }
    
    
}
