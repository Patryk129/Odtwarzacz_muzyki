using System.Collections.ObjectModel;

namespace Odtwarzacz_muzyki;

public partial class SongPage : ContentPage
{
    private ObservableCollection<Song> _songs = new();
    private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
    private int _currentIndex = -1;
    public SongPage()
	{
		InitializeComponent();
    }
    private async void Previous_Clicked(object sender, EventArgs e)
    {

    }
    private async void Play_Clicked(object sender, EventArgs e)
    {

    }
    private async void Next_Clicked(object sender, EventArgs e)
    {

    }





    private async void Files_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilesPage(_songs));
    }
    private async void Home_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new SongPage(_songs));
    }
    private async void Search_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//SearchPage");
        await Navigation.PushAsync(new SearchPage(_songs));

    }

    private async void Account_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AccountPage");
    }
}