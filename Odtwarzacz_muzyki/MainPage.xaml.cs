using System.Collections.ObjectModel;

namespace Odtwarzacz_muzyki;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Playlist> Playlists => PlaylistManager.Playlists;
    private ObservableCollection<Song> _allSongs = new();

    public MainPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _allSongs = songs;

        PlaylistsCollectionView.ItemsSource = Playlists;
    }

    private async void OpenPlaylist_Clicked(object sender, EventArgs e)
    {
        var playlist = (sender as Button)?.BindingContext as Playlist;
        if (playlist == null) return;

        // Tworzymy SongPage z utworami wybranej playlisty
        var songPage = new SongPage(playlist.Songs);
        await Navigation.PushAsync(songPage);
    }

    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    private async void Search_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SearchPage(_allSongs));
    }

    private async void Files_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilesPage(_allSongs));
    }

    private async void Account_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PlaylistPage(_allSongs));
    }
}
