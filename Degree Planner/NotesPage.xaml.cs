using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace Degree_Planner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotesPage : ContentPage
    {
        public Database.Course Course = new Database.Course();
        public NotesPage()
        {
            InitializeComponent();
        }
        public NotesPage(Database.Course course)
        {
            InitializeComponent();
            Course = course;
            courseNameText.Text = Course.CourseName;
            notesText.Text = Course.CourseNotes;
        }

        private async void shareButton_Clicked(object sender, EventArgs e)
        {
            await Share.RequestAsync(notesText.Text);
        }

        private async void backButton_Clicked(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Caution", "All unsaved data will be lost. Do you wish to continue?", "Yes", "No");
            if (answer)
            {
                await Navigation.PopAsync();
            }
            
        }

        private void saveButton_Clicked(object sender, EventArgs e)
        {
            using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
            {
                Course.CourseNotes = notesText.Text;
                conn.Update(Course);
            }
        }
    }
}