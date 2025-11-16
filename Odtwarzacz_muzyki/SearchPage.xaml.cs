using System.Collections.ObjectModel;

namespace Odtwarzacz_muzyki;

public partial class SearchPage : ContentPage
{
    private readonly ObservableCollection<Song> _allSongs;
    private readonly ObservableCollection<Song> _filteredSongs = new();

    public SearchPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _allSongs = songs;

        SearchResults.ItemsSource = _filteredSongs;
        RefreshResults("");
    }

    private void Search_TextChanged(object sender, TextChangedEventArgs e)
    {
        RefreshResults(e.NewTextValue);
    }

    private void RefreshResults(string query)
    {
        query = query?.ToLower() ?? "";

        _filteredSongs.Clear();

        foreach (var song in _allSongs)
        {
            if (song.Title?.ToLower().Contains(query) == true ||
                song.Author?.ToLower().Contains(query) == true)
            {
                _filteredSongs.Add(song);
            }
        }
    }

    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage(_allSongs));
    }

    private async void Search_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SearchPage");
    }

    private async void Files_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilesPage(_allSongs));
    }

    private async void Account_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//AccountPage");
        await Navigation.PushAsync(new PlaylistPage(_allSongs));
    }
}
