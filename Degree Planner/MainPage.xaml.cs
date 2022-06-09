using Degree_Planner.Database;
using System.Collections.Generic;
using Xamarin.Forms;



namespace Degree_Planner
{
    public partial class MainPage : ContentPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Terms = Term.GetTerms();
            TermsListView.ItemsSource = Terms;
        }
        public static List<Term> Terms = new List<Term>();
        public Term selectedTerm = new Term();
        public MainPage()
        {
            InitializeComponent();

        }
        public MainPage(string databaseLocation)
        {
            InitializeComponent();
            Terms = Term.GetTerms();
            TermsListView.ItemsSource = Terms;
        }

        private void TermsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selectedTerm = TermsListView.SelectedItem as Term;
        }

        private void viewEdit_Clicked(object sender, System.EventArgs e)
        {
            if (TermsListView.SelectedItem == null) { DisplayAlert("Notification", "Please select a term first", "Ok"); }
            else
            {
                Navigation.PushAsync (new TermView(selectedTerm));
            }
        }

        private void addTerm_Clicked(object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new TermView());
        }

        private async void deleteTerm_Clicked(object sender, System.EventArgs e)
        {
            if (TermsListView.SelectedItem == null) { await DisplayAlert("Notification", "Please select a term to delete", "Ok"); }
            else
            {
                var confirmed = await DisplayAlert("Confirm", "Are you sure you want to permanently delete the selected term?", "Yes", "Cancel");
                if (confirmed)
                {
                    if (Course.GetCourses(selectedTerm.TermID).Count == 0)
                    {
                        using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DatabaseLocation))
                        {
                            conn.Delete(selectedTerm);
                            await DisplayAlert("Notification", $"{selectedTerm.TermName} has been removed", "Ok");
                            Terms = Term.GetTerms();
                            TermsListView.ItemsSource = Terms;
                        }
                    }
                    else
                    {
                        await DisplayAlert("Notification", "The term must not have any courses before you can delete it.", "Ok");
                    }
                }
            }
        }
    }
}
