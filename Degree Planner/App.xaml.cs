using SQLite;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Degree_Planner
{
    public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());

        }
        public App(string databaseLocation)
        {
            InitializeComponent();
            DatabaseLocation = databaseLocation;
            var conn = new SQLiteConnection(DatabaseLocation);
            conn.CreateTable<Database.Term>();
            conn.CreateTable<Database.Course>();
            conn.CreateTable<Database.Assessment>();
            conn.CreateTable<Database.Notification>();
            var terms = conn.Table<Database.Term>().ToList();
            if (!terms.Any())
            {
                Database.Term tempTerm = new Database.Term();
                tempTerm.TermName = "Term 1";
                tempTerm.startDate = DateTime.Now.Date.AddDays(-10);
                tempTerm.endDate = DateTime.Now.Date.AddDays(10);

                Database.Course tempCourse = new Database.Course();
                tempCourse.TermID = 1;
                tempCourse.CourseName = "Mobile Application Development Using C#";
                tempCourse.CourseStatus = "In Progress";
                tempCourse.CourseStart = DateTime.Now.Date.AddDays(-9);
                tempCourse.CourseEnd = DateTime.Now.Date.AddDays(9);
                tempCourse.InstructorName = "Aaron DeBord";
                tempCourse.InstructorPhone = "206-877-3911";
                tempCourse.InstructorEmail = "adebor3@wgu.edu";
                tempCourse.CourseNotes = "This is where my notes would go";
                

                Database.Assessment tempAssessment = new Database.Assessment();
                tempAssessment.CourseID = 1;
                tempAssessment.ObjectiveName = "Mobile Application";
                tempAssessment.ObjectiveEnd = DateTime.Now.Date.AddDays(9);
                tempAssessment.PerformanceName = "Mobile Test";
                tempAssessment.PerformanceEnd = DateTime.Now.Date.AddDays(10);
                
                
                conn.Insert(tempTerm);
                conn.Insert(tempCourse);
                conn.Insert(tempAssessment);
                conn.Close();

            }
            MainPage = new NavigationPage(new MainPage(databaseLocation));

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
