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

        public SongPage(ObservableCollection<Song> playlistSongs)
        {
            InitializeComponent();
            _songs = playlistSongs;
            _currentIndex = 0;
            UpdateSongInfo();
        }
        public SongPage() : this(new ObservableCollection<Song>())
        {
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
        private void Play()
        {
            try
            {
                if (_songs.Count == 0) return;
                if (_currentIndex == -1) _currentIndex = 0;

                var path = _songs[_currentIndex].Path;
                if (string.IsNullOrEmpty(path)) return;

                // Jeœli player nie ma jeszcze Ÿród³a — ustaw tylko raz
                if (Player.Source == null)
                    Player.Source = path;

                if (!_isPlaying)
                {
                    // Wznów lub rozpocznij odtwarzanie
                    Player.Play();
                    PlayButton.Source = "pause_icon.png";
                    _isPlaying = true;
                }
                else
                {
                    // Pauza (nie stop!)
                    Player.Pause();
                    PlayButton.Source = "play_icon.png";
                    _isPlaying = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d: {ex.Message}");
            }
        }
        private void PlayButton_Clicked(object sender, EventArgs e)
        {
            Play();
        
        
        }



        private async void Previous_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0)
                _currentIndex = _songs.Count - 1;

            UpdateSongInfo();
            //await Play();
        }

        private async void Next_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _songs.Count)
                _currentIndex = 0;

            UpdateSongInfo();
            //await PlayButton_Clicked();
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
            await Navigation.PushAsync(new PlaylistPage(_songs));
        }

        public void SetCurrentSong(Song song)
        {
            _currentIndex = _songs.IndexOf(song);
            if (_currentIndex < 0) _currentIndex = 0;
            UpdateSongInfo();
        }

    }
}
