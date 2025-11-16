using System.Collections.ObjectModel;
using System.Text.Json;

namespace Odtwarzacz_muzyki;

public partial class MainPage : ContentPage
{
    public ObservableCollection<Playlist> Playlists => PlaylistManager.Playlists;
    private ObservableCollection<Song> _allSongs = new();
    private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
    public MainPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _allSongs = songs;
        _ = LoadAsync();
    }
    public MainPage()
    {
        InitializeComponent();
        _ = LoadAsync();
    }
    private async Task LoadAsync()
    {
        await LoadSongsAsync();    
        await PlaylistManager.LoadAsync(); 
        foreach (var playlist in Playlists)
        {
            var syncedSongs = new ObservableCollection<Song>();
            foreach (var song in playlist.Songs)
            {
                var match = _allSongs.FirstOrDefault(s => s.Path == song.Path);
                if (match != null)
                    syncedSongs.Add(match);
            }
            playlist.Songs = syncedSongs;
        }
        if (!Playlists.Any(p => p.Name == "Wszystkie utwory"))
        {
            var allSongsPlaylist = new Playlist
            {
                Name = "Wszystkie utwory",
                Songs = new ObservableCollection<Song>(_allSongs),
                ImagePath = "default_playlist.png"
            };
            Playlists.Insert(0, allSongsPlaylist);
        }

        PlaylistsCollectionView.ItemsSource = Playlists;
    }
    private async Task LoadSongsAsync()
    {
        try
        {
            if (File.Exists(_songsFilePath))
            {
                var json = await File.ReadAllTextAsync(_songsFilePath);
                var loadedSongs = JsonSerializer.Deserialize<ObservableCollection<Song>>(json);

                if (loadedSongs != null)
                {
                    _allSongs.Clear();
                    foreach (var song in loadedSongs)
                        _allSongs.Add(song);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd odczytu piosenek: {ex.Message}");
        }
    }

    private async void OpenPlaylist_Clicked(object sender, EventArgs e)
    {
        var playlist = (sender as Button)?.BindingContext as Playlist;
        if (playlist == null) return;

        var songPage = new SongPage(playlist.Songs);
        await Navigation.PushAsync(songPage);
    }

    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
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
