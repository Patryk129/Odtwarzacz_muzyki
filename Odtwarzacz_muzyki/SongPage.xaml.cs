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

            Player.MediaEnded += Player_MediaEnded; 
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
            if (_currentIndex < 0 || _currentIndex >= _songs.Count) return;

            var song = _songs[_currentIndex];
            BindingContext = song;
            if (GlobalPlayer.Instance != null)
            {
                GlobalPlayer.Instance.SongTitleLabel.Text = song.Title;
                GlobalPlayer.Instance.PlayerControl.Source = song.Path;
            }

            if (!string.IsNullOrEmpty(song.Path) && File.Exists(song.Path))
            {
                Player.Source = song.Path;
            }
        }

        private void Play()
        {
            if (_songs.Count == 0 || _currentIndex < 0) return;

            if (!_isPlaying)
            {
                Player.Play();
                PlayButton.Source = "pause_icon.png";
                _isPlaying = true;
            }
            else
            {
                Player.Pause();
                PlayButton.Source = "play_icon.png";
                _isPlaying = false;
            }
        }

        private void PlayCurrentSongWithoutChangingState()
        {
            if (_songs.Count == 0 || _currentIndex < 0) return;

            var song = _songs[_currentIndex];
            if (string.IsNullOrEmpty(song.Path) || !File.Exists(song.Path)) return;

            Player.Source = song.Path;
            BindingContext = song;

            if (_isPlaying)
                Player.Play();
        }

        private void Player_MediaEnded(object sender, EventArgs e)
        {
            _currentIndex++;
            if (_currentIndex >= _songs.Count) _currentIndex = 0;

            UpdateSongInfo();
            PlayCurrentSongWithoutChangingState();
        }

        private void Previous_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _songs.Count - 1;

            UpdateSongInfo();
            PlayCurrentSongWithoutChangingState();
        }

        private void PlayButton_Clicked(object sender, EventArgs e)
        {
            Play();
        }

        private void Next_Clicked(object sender, EventArgs e)
        {
            if (_songs.Count == 0) return;

            _currentIndex++;
            if (_currentIndex >= _songs.Count) _currentIndex = 0;

            UpdateSongInfo();
            PlayCurrentSongWithoutChangingState();
        }
        public void SetCurrentSong(Song song)
        {
            _currentIndex = _songs.IndexOf(song);
            if (_currentIndex < 0) _currentIndex = 0;
            UpdateSongInfo();
            PlayCurrentSongWithoutChangingState();
        }


        private async void Home_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage(_songs));
        }
        private async void Files_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FilesPage(_songs));
        }
        private async void Playlist_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlaylistPage(_songs));
        }
        private async void Search_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SearchPage(_songs));
        }

        
    }
}
