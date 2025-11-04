using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Odtwarzacz_muzyki
{
    public partial class GlobalPlayer : ContentView
    {
        private static GlobalPlayer _instance;
        private ObservableCollection<Song> _songs = new();
        private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
        private int _currentIndex = -1;
        //private int _currentIndex = 0;

        public static GlobalPlayer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GlobalPlayer();
                return _instance;
            }
        }

        public GlobalPlayer()
        {
            InitializeComponent();
            _ = LoadSongsAsync();
        }

        //public void PlaySong(string title, string path)
        //{
        //    TitleLabel.Text = title;
        //    Player.Source = path;
        //    Player.Play();
        //    _isPlaying = true;
        //    PlayPauseButton.Source = "pause_icon.png";
        //}
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
                    SongTitle.Text = _songs[_currentIndex].Title;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"B³¹d odczytu piosenek: {ex.Message}");
            }
        }
        private bool _isPlaying = false;

        private void PlayPauseButton_Clicked(object sender, EventArgs e)
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
    }
}
