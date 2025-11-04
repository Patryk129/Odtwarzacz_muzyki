using System.Collections.ObjectModel;
using System.Text.Json;

namespace Odtwarzacz_muzyki;

public partial class SearchPage : ContentPage
{
    private readonly ObservableCollection<Song> _songs;
    private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
    private int _currentIndex = -1;
    public SearchPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _songs = songs;
        
        //SongsCollectionView.ItemsSource = _songs;
        _ = LoadSongsAsync();
    }
    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
    private async void Search_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//SearchPage");
    }
    
    private async void Account_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//AccountPage");
    }
    private void PlayButton_Clicked(object sender, EventArgs e)
    {
        if (_songs.Count == 0) return;
        if (_currentIndex == -1) _currentIndex = 0;

        Player.Source = _songs[_currentIndex].Path;
        Player.Play();

        //SongTitle.Text = $"▶ {_songs[_currentIndex].Title}";
    }
    private async Task SaveSongsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_songs);
            await File.WriteAllTextAsync(_songsFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd zapisu piosenek: {ex.Message}");
        }
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
                    _songs.Clear();
                    foreach (var song in loadedSongs)
                        _songs.Add(song);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd odczytu piosenek: {ex.Message}");
        }
    }
    private async void Files_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new FilesPage(_songs));
    }
}