using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Degree_Planner.Database;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;

namespace Degree_Planner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CoursePage : ContentPage
    {
        
        public List<string> statusList = new List<string>();
        public Term SelectedTerm = new Term();
        public Course SelectedCourse = new Course();
        public string pageState = String.Empty;
        public bool validateData()
        {
            bool result = true;
            if (String.IsNullOrEmpty(courseNameText.Text)) 
            { 
                result = false;
                DisplayAlert("Notification", "You must enter a class name", "Ok");
            }
            try
            {
                var email = new System.Net.Mail.MailAddress(instructorEmail.Text);
                
            }
            catch 
            { 
                result = false;
                DisplayAlert("Notification", "The instructor Email is not valid", "Ok");
            }
            if (String.IsNullOrEmpty(instructorPhone.Text)) 
            { 
                result = false;
                DisplayAlert("Notification", "You must fill in the instuctors phone number", "Ok");
            }
            if (String.IsNullOrEmpty(instructorName.Text))
            {
                result = false;
                DisplayAlert("Notification", "You must fill in the instructors name", "Ok");
            }
            if (statusPicker.SelectedItem == null)
            {
                result = false;
                DisplayAlert("Notification", "You must selcet a course status", "Ok");
            }
            if(startDateDP.Date > endDateDP.Date)
            {
                result = false;
                DisplayAlert("Notification", "The class start date must be before the end date", "Ok");
            }
            return result;
        }
        public void saveData()
        {
            SelectedCourse.CourseName = courseNameText.Text;
            SelectedCourse.CourseStatus = statusPicker.SelectedItem.ToString();
            SelectedCourse.TermID = SelectedTerm.TermID;
            SelectedCourse.CourseStart = startDateDP.Date;
            SelectedCourse.CourseEnd = endDateDP.Date;
            SelectedCourse.InstructorEmail = instructorEmail.Text;
            SelectedCourse.InstructorName = instructorName.Text;
            SelectedCourse.InstructorPhone = instructorPhone.Text;
            //SelectedCourse.StartNotification = startNotificationCheck.IsChecked;
            //SelectedCourse.EndNotification = endNotificationCheck.IsChecked;
            //SelectedCourse.startNotificationDate = startNotificationDP.Date;
            //SelectedCourse.endNotificationDate = endNotificationDP.Date;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

        }
        public void update()
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
            {
                
                SelectedCourse = conn.Table<Course>().FirstOrDefault(x => x.CourseID == SelectedCourse.CourseID);
               
            }
            termNameText.Text = SelectedTerm.TermName;
            courseNameText.Text = SelectedCourse.CourseName;
            startDateDP.Date = SelectedCourse.CourseStart;
            endDateDP.Date = SelectedCourse.CourseEnd;
            statusPicker.ItemsSource = statusList;
            statusPicker.SelectedItem = SelectedCourse.CourseStatus;
            instructorName.Text = SelectedCourse.InstructorName;
            instructorPhone.Text = SelectedCourse.InstructorPhone;
            instructorEmail.Text = SelectedCourse.InstructorEmail;
        }
        public CoursePage()
        {
            InitializeComponent();
        }
        public void populateStatus()
        {
            statusList.Add("In Progress");
            statusList.Add("Completed");
            statusList.Add("Dropped");
            statusList.Add("Plan to take");
        }
        public CoursePage(Term selectedTerm)
        {
            InitializeComponent();
            populateStatus();
            SelectedTerm = selectedTerm;
            pageState = "add";
            termNameText.Text = SelectedTerm.TermName;
            startDateDP.Date = DateTime.Now;
            endDateDP.Date = DateTime.Now;
            //startNotificationDP.Date = DateTime.Now;
            //endNotificationDP.Date = DateTime.Now;
            SelectedCourse.TermID = SelectedTerm.TermID;
            statusPicker.ItemsSource = statusList;

        }
        public CoursePage(Term selectedTerm, Course selectedCourse)
        {
            InitializeComponent();
            populateStatus();
            pageState = "viewEdit";
            SelectedTerm = selectedTerm;
            SelectedCourse = selectedCourse;
            update();
        }

        private void notesButton_Clicked(object sender, EventArgs e)
        {
            if (pageState == "viewEdit")
            {
                Navigation.PushAsync(new NotesPage(SelectedCourse));
                
            }
            else
            {
                DisplayAlert("Notification", "You must save the new class before you can add notes", "Ok");
            }

        }

        private void assessmentsButton_Clicked(object sender, EventArgs e)
        {
            if (pageState == "add")
            {
                DisplayAlert("Notification", "You must save the new class before you can add assessments", "Ok");
            }
            else
            {
                Course temp = SelectedCourse;
                Navigation.PushAsync(new AssessmentsPage(temp));
            }
        }

        private void saveButton_Clicked(object sender, EventArgs e)
        {
            if(pageState == "viewEdit")
            {
                if (validateData() == true)
                {
                    saveData();
                    using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.Update(SelectedCourse);
                        
                    }
                    Navigation.PopAsync();
                    Navigation.PushAsync(new CoursePage(SelectedTerm, SelectedCourse));
                }
            }
            if (pageState == "add")
            {
                if (validateData() == true)
                {
                    Course temp = new Course();
                    int tempid;
                    saveData();
                    using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                    {
                        
                        conn.BeginTransaction();
                        conn.Insert(SelectedCourse);
                        tempid = conn.ExecuteScalar<int>("SELECT last_insert_rowid()");
                        temp = conn.Table<Course>().FirstOrDefault(x => x.CourseID == tempid);
                        Assessment tempAssessment = new Assessment();
                        tempAssessment.CourseID = tempid;
                        conn.Insert(tempAssessment);
                        conn.Commit();
                        conn.Close();
                    }
                    Navigation.PopAsync();
                    Navigation.PushAsync(new CoursePage(SelectedTerm, temp));
                }
                
            }
        }

        private async void backButton_Clicked(object sender, EventArgs e)
        {
            var confirm = await DisplayAlert("All unsaved data will be lost", "Are you sure you want to go back?", "Yes", "No");
            if (confirm)
            {
                await Navigation.PopAsync();
            }
        }

        private void endNotificationButton_Clicked(object sender, EventArgs e)
        {
            DateTime tempDate = endDateDP.Date;
            Navigation.PushAsync(new SetNotification(SelectedCourse, tempDate, "end"));
        }

        private void startNotificationButton_Clicked(object sender, EventArgs e)
        {
            DateTime tempDate = startDateDP.Date;
            Navigation.PushAsync(new SetNotification(SelectedCourse, tempDate, "start"));
        }
    }
}