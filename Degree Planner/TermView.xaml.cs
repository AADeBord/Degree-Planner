using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Degree_Planner.Database;

namespace Degree_Planner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermView : ContentPage
    {
        public Term selectedTerm = new Term();
        public Course selectedCourse = new Course();
        public string pageCondition = String.Empty;
        List<Course> courses = new List<Course>();
        int classNumber;


        protected override void OnAppearing()
        {
            base.OnAppearing();
            courses = Course.GetCourses(selectedTerm.TermID);
            classListView.ItemsSource = courses;
            classNumber = courses.Count;
        }
        public TermView()
        {
            InitializeComponent();
            pageCondition = "addTerm";
            startDateDP.Date = DateTime.Now;
            endDateDP.Date = DateTime.Now;
            addCourse.IsEnabled = false;
            viewEditCourse.IsEnabled = false;
            deleteCourse.IsEnabled = false;
            
            
        }
        public TermView(Term term)
        {
            InitializeComponent();
            pageCondition = "viewEdit";
                   
            
            selectedTerm = term;
            termNameText.Text = selectedTerm.TermName;
            startDateDP.Date = selectedTerm.startDate;
            endDateDP.Date = selectedTerm.endDate;
            courses = Course.GetCourses(term.TermID);
            classListView.ItemsSource = courses;
            classNumber = courses.Count;
            
        }
        public bool validateNameDate()
        {
            bool result = true;
            if (String.IsNullOrEmpty(termNameText.Text) == true)
            {
                DisplayAlert("Notification", "You must enter a name for the term.", "Ok");
                result = false;
            }
            if (startDateDP.Date > endDateDP.Date)
            {
                DisplayAlert("Notification", "Your start date cannot be after your end date", "Ok");
                result = false;
            }
            if (pageCondition == "addTerm")
            {
                if (MainPage.Terms.Count > 0)
                {
                    if (MainPage.Terms[MainPage.Terms.Count - 1].endDate > startDateDP.Date)
                    {
                        DisplayAlert("Notification", "You can't start a term while another term is in progress", "Ok");
                        result = false;
                    }
                }
            }
            return result;
        }

        private void classListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedCourse = classListView.SelectedItem as Course;
        }

        private void viewEditCourse_Clicked(object sender, EventArgs e)
        {
            if (classListView.SelectedItem == null)
            {
                DisplayAlert("Notification", "You must select a course first.", "Ok");
            }
            else
            {
                Navigation.PushAsync(new CoursePage(selectedTerm, selectedCourse));
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

        private void saveButton_Clicked(object sender, EventArgs e)
        {
            if (pageCondition == "addTerm")
            {
                if (validateNameDate() == true)
                {
                    using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                    {
                        Database.Term tempTerm = new Database.Term();
                        tempTerm.TermName = termNameText.Text;
                        tempTerm.startDate = startDateDP.Date;
                        tempTerm.endDate = endDateDP.Date;
                        conn.Insert(tempTerm);
                    }
                    MainPage.Terms = Term.GetTerms();
                    Navigation.PopAsync();
                    Navigation.PushAsync(new TermView(MainPage.Terms[MainPage.Terms.Count - 1]));
                    DisplayAlert("Notification", "New term saved successfuly", "Ok");


                }
            }
            if (pageCondition == "viewEdit")
            {
                int termId = selectedTerm.TermID;
                if (validateNameDate() == true)
                {
                    selectedTerm.startDate = startDateDP.Date;
                    selectedTerm.endDate = endDateDP.Date;
                    selectedTerm.TermName = termNameText.Text;

                    using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                    {
                        conn.Update(selectedTerm);
                    }
                    MainPage.Terms = Term.GetTerms();
                    selectedTerm = MainPage.Terms.First(a => a.TermID == termId);
                    Navigation.PopAsync();
                    Navigation.PushAsync(new TermView(selectedTerm));
                }
            }
        }

        private void addCourse_Clicked(object sender, EventArgs e)
        {
            if (pageCondition == "addTerm")
            {
                DisplayAlert("Notification", "Please save the term before adding courses", "Ok");
            }
            else
            {
                if (classNumber >= 6)
                {
                    DisplayAlert("Notification", "Only 6 courses allowed per term. Unable to add another course", "Ok");
                }
                else { Navigation.PushAsync(new CoursePage(selectedTerm)); }
            }
        }

        private void deleteCourse_Clicked(object sender, EventArgs e)
        {
            if(classListView.SelectedItem == null)
            {
                DisplayAlert("Notification", "You must select a course to delete", "Ok");
            }
            else
            {
                using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                {
                    conn.Delete(selectedCourse);
                    conn.Delete<Assessment>(selectedCourse.CourseID);

                }
                courses = Course.GetCourses(selectedTerm.TermID);
                classListView.ItemsSource = courses;
            }
        }
    }
}