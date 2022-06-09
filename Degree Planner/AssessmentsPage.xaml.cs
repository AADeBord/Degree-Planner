using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Degree_Planner.Database;
using Plugin.LocalNotifications;

namespace Degree_Planner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentsPage : ContentPage
    {
        public Course currentCourse = new Course();
        public Assessment currentAssessment = new Assessment();
        DateTime testDate = new DateTime();
        public AssessmentsPage()
        {
            InitializeComponent();
        }
        public AssessmentsPage(Course course)
        {
            InitializeComponent();
            currentCourse = course;

            update();
            setDates();
            
        }
        public void update()
        {
            courseNameText.Text = currentCourse.CourseName;
            currentAssessment = Assessment.GetAssessment(currentCourse.CourseID);
            nameTextOA.Text = currentAssessment.ObjectiveName;
            nameTextPA.Text = currentAssessment.PerformanceName;
            //endNotificationCheckOA.IsChecked = currentAssessment.ObjectiveNotification;
            //endNotificationCheckPA.IsChecked = currentAssessment.PerformanceNotification;
            setDates();
        }
        public void setDates()
        {
            if (currentAssessment.PerformanceEnd == testDate) { endDatePickerOA.Date = DateTime.Now; }
            else { endDatePickerOA.Date = currentAssessment.PerformanceEnd; }
            if (currentAssessment.ObjectiveEnd == testDate) { endDatePickerOA.Date = DateTime.Now; }
            else { endDatePickerOA.Date = currentAssessment.ObjectiveEnd; }
            //if (currentAssessment.ObjectiveNotificationDate == testDate) { endNotificationOADate.Date = DateTime.Now; }
            //else { endNotificationOADate.Date = currentAssessment.ObjectiveNotificationDate; }
            //if (currentAssessment.PerformanceNotificationDate == testDate) { endNotificationPADate.Date = DateTime.Now; }
            //else { endNotificationPADate.Date = currentAssessment.PerformanceNotificationDate; }
        }
        public void saveDataPA()
        {
            currentAssessment.PerformanceName = nameTextPA.Text;
            currentAssessment.PerformanceEnd = endDatePickerPA.Date;
            //currentAssessment.PerformanceNotificationDate = endNotificationPADate.Date;
            //currentAssessment.PerformanceNotification = endNotificationCheckPA.IsChecked;
        }
        public void saveDataOA()
        {
            currentAssessment.ObjectiveName = nameTextOA.Text;
            currentAssessment.ObjectiveEnd = endDatePickerOA.Date;
            //currentAssessment.ObjectiveNotificationDate = endNotificationOADate.Date;
            //currentAssessment.ObjectiveNotification = endNotificationCheckOA.IsChecked;
        }
        public void deleteDataOA()
        {
            currentAssessment.ObjectiveName = "None";
            currentAssessment.ObjectiveEnd = testDate;
            currentAssessment.ObjectiveNotificationDate = testDate;
            currentAssessment.ObjectiveNotification = false;
        }
        public void deleteDataPA()
        {
            currentAssessment.PerformanceName = "None";
            currentAssessment.PerformanceEnd = testDate;
            currentAssessment.PerformanceNotificationDate = testDate;
            currentAssessment.PerformanceNotification = false;
        }

        private void saveButtonPA_Clicked(object sender, EventArgs e)
        {
            saveDataPA();
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
            {
                conn.Update(currentAssessment);
            }
            DisplayAlert("Notification", "Performance Assessment Saved", "Ok");

        }

        private void deletePA_Clicked(object sender, EventArgs e)
        {
            deleteDataPA();
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
            {
                conn.Update(currentAssessment);
            }
            DisplayAlert("Notification", "Performance assessment deleted", "Ok");
            update();
        }

        private void saveButtonOA_Clicked(object sender, EventArgs e)
        {
            saveDataOA();
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
            {
                conn.Update(currentAssessment);
            }
            DisplayAlert("Notification", "Objective Assessment Saved", "Ok");
            
        }

        private void deleteOA_Clicked(object sender, EventArgs e)
        {
            deleteDataOA();
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
            {
                conn.Update(currentAssessment);
            }
            DisplayAlert("Notification", "Objective assessment deleted", "Ok");
            update();
        }

        private async void goBack_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("All unsaved data will be lost", "Continue?", "Yes", "No");
            if (answer)
            {
               await Navigation.PopAsync();
            }
        }

        private void notificationOAButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SetNotification(currentAssessment, endDatePickerOA.Date, courseNameText.Text, nameTextOA.Text, "OA"));
        }

        private void notificationPAButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SetNotification(currentAssessment, endDatePickerPA.Date, courseNameText.Text, nameTextPA.Text, "PA"));
        }
    }
}