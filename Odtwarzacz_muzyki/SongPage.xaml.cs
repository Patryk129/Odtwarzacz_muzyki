using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Odtwarzacz_muzyki
{
    public partial class SongPage : ContentPage
    {
        private ObservableCollection<Song> _songs = new();
        private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
        private int _currentIndex = -1;
        private bool _isPlaying = false;

        public SongPage()
        {
            InitializeComponent();
            _ = LoadSongsAsync();
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

                if (_songs.Count > 0)
                {
                    _currentIndex = 0;
                    UpdateSongInfo();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d odczytu piosenek: {ex.Message}");
            }
        }

        private void UpdateSongInfo()
        {
            if (_currentIndex < 0 || _currentIndex >= _songs.Count)
                return;

            var song = _songs[_currentIndex];

            BindingContext = song;

            if (GlobalPlayer.Instance != null)
            {
                GlobalPlayer.Instance.SongTitleLabel.Text = song.Title;
                GlobalPlayer.Instance.PlayerControl.Source = song.Path;
            }
        }

        private async Task PlayCurrentSongAsync()
        {
            try
            {
                if (_songs.Count == 0 || _currentIndex == -1)
                    return;

                var song = _songs[_currentIndex];
                var player = GlobalPlayer.Instance.Player;

                player.Source = song.Path;
                player.Play();

                _isPlaying = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d odtwarzania: {ex.Message}");
            }
        }

        private async void Previous_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _songs.Count - 1;

            UpdateSongInfo();
            await PlayCurrentSongAsync();
        }

        private async void Play_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0 || _currentIndex == -1)
                return;

            var player = GlobalPlayer.Instance.Player;

            if (!_isPlaying)
            {
                await PlayCurrentSongAsync();
                _isPlaying = true;
            }
            else
            {
                player.Pause();
                _isPlaying = false;
            }
        }

        private async void Next_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _songs.Count)
                _currentIndex = 0;

            UpdateSongInfo();
            await PlayCurrentSongAsync();
        }

        private async void Files_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FilesPage(_songs));
        }

        private async void Home_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SongPage());
        }

        private async void Search_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchPage(_songs));
        }

        private async void Account_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//AccountPage");
        }
    }
}
