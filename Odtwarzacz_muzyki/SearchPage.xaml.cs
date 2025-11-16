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
        if (query != null)
        {
            query = query.ToLower();
        }
        else
        {
            query = "";
        }

        _filteredSongs.Clear();

        foreach (var song in _allSongs)
        {
            bool titleMatches = false;
            bool authorMatches = false;

            if (song.Title != null)
            {
                titleMatches = song.Title.ToLower().Contains(query);
            }

            if (song.Author != null)
            {
                authorMatches = song.Author.ToLower().Contains(query);
            }

            if (titleMatches || authorMatches)
            {
                _filteredSongs.Add(song);
            }
        }
    }

    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage(_allSongs));
    }
    private async void Files_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilesPage(_allSongs));
    }
    private async void Playlist_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PlaylistPage(_allSongs));
    }
    private async void Search_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SearchPage(_allSongs));
    }
}
