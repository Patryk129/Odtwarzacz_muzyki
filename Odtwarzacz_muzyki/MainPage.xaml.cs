namespace Odtwarzacz_muzyki
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Home_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
        private async void Search_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//SearchPage");
        }
        private async void Files_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//FilesPage");
        }
        private async void Account_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AccountPage");
        }
    }
}
