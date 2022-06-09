using Degree_Planner.Database;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.LocalNotifications;

namespace Degree_Planner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetNotification : ContentPage
    {
        string noteType = String.Empty;
        public Assessment tempAssessment = new Assessment();
        public Course tempCourse = new Course();
        string tempCourseName = String.Empty;
        DateTime tempDate = new DateTime();
        public SetNotification()
        {
            InitializeComponent();
        }
        public SetNotification(Assessment assessment, DateTime date, string courseName, string assessmentName, string type)
        {
            InitializeComponent();
            tempAssessment = assessment;
            tempCourseName = courseName;
            tempDate = date;
            assessmentPopulate(type);
        }
        public SetNotification(Course course, DateTime date, string type)
        {
            InitializeComponent();
            tempCourse = course;
            tempDate = date;
            coursePopulate(type);
        }
        public void coursePopulate(string type)
        {
            if (type == "start")
            {
                labelText.Text = tempCourse.CourseName;
                notificationName.Text = "Course start notification";
                notificationDate.Date = tempCourse.CourseStart;
                noteType = "CourseStart";
            }
            else
            {
                labelText.Text = tempCourse.CourseName;
                notificationName.Text = "Course end notification";
                notificationDate.Date = tempCourse.CourseEnd;
                noteType = "CourseEnd";
            }

        }
        public void assessmentPopulate(string type)
        {
            if (type == "OA")
            {
                labelText.Text = tempCourseName;
                notificationType.Text = "Objective Assessment";
                notificationName.Text = tempAssessment.ObjectiveName;
                notificationDate.Date = tempDate;
                noteType = "AssessmentOA";
            }
            else
            {
                labelText.Text = tempCourseName;
                notificationType.Text = "Performance Assessment";
                notificationName.Text = tempAssessment.PerformanceName;
                notificationDate.Date = tempDate;
                noteType = "AssessmentPA";
            }
        }

        private void cancle_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
        public string NoteString()
        {
            string result = String.Empty;
            if (noteType == "CourseStart")
            {
                result = $"{tempCourse.CourseName} is going to start on {tempCourse.CourseStart.ToString("MM/dd/yyyy")}";

            }
            if (noteType == "CourseEnd")
            {
                result = $"{tempCourse.CourseName} is going to end on {tempCourse.CourseEnd.ToString("MM/dd/yyyy")}";
            }
            if (noteType == "AssessmentOA")
            {
                result = $"The objective assessment {tempAssessment.ObjectiveName} for {tempCourseName} " +
                    $"is due on {tempAssessment.ObjectiveEnd.ToString("MM/dd/yyyy")} ";
            }
            if (noteType == "AssessmentPA")
            {
                result = $"The performance assessment {tempAssessment.PerformanceName} for {tempCourseName} " +
                    $"is due on {tempAssessment.PerformanceEnd.ToString("MM/dd/yyyy")} ";
            }
            return result;
        }

        private async void setNotification_Clicked(object sender, EventArgs e)
        {
            DateTime noteTime = notificationDate.Date.Add(notificationTime.Time);
            
            

            var answer = await DisplayAlert("Alert", $"Set notification to appear on { notificationDate.Date.ToString("MM/dd/yyyy")} at " +
             $"{notificationTime.Time.ToString()}?", "Yes", "No");
            if (answer)
            {
                int tempid;
                Notification notification = new Notification();
                notification.NoteMessage = NoteString();
                notification.NoteTime = noteTime;
                //this is to create a message id number.
                using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                {
                    conn.BeginTransaction();
                    conn.CreateTable<Notification>();
                    conn.Insert(notification);
                    tempid = conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
                    notification = conn.Table<Notification>().FirstOrDefault(x => x.NoteID == tempid);
                    conn.Commit();
                    conn.Close();
                }
                  
                CrossLocalNotifications.Current.Show("Notification", NoteString(), tempid, noteTime);
                await Navigation.PopAsync();
            }
        }
    }
}