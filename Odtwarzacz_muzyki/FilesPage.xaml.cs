using System.Collections.ObjectModel;
using System.Text.Json;

namespace Odtwarzacz_muzyki;

public partial class FilesPage : ContentPage
{
    private readonly ObservableCollection<Song> _songs;
    private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
    private int _currentIndex = -1;
    public FilesPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _songs = songs; // przypisanie przekazanej kolekcji do pola
        SongsCollectionView.ItemsSource = _songs;
        _ = LoadSongsAsync();
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
        //await Shell.Current.GoToAsync("//FilesPage");
        try
        {
            var result = await FilePicker.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "Wybierz pliki muzyczne",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".flac" } },
                        { DevicePlatform.Android, new[] { "audio/*" } },
                        { DevicePlatform.iOS, new[] { "public.audio" } }
                    })
            });

            if (result != null)
            {
                foreach (var file in result)
                {
                    _songs.Add(new Song
                    {
                        Title = System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                        Path = file.FullPath
                    });
                }
            }
            await SaveSongsAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Błąd", $"Nie udało się wybrać plików: {ex.Message}", "OK");
        }
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
    private bool _isPlaying = false;
    //private void PlayButton_Clicked(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (_songs.Count == 0) return;
    //        if (_currentIndex == -1) _currentIndex = 0;

    //        var path = _songs[_currentIndex].Path;
    //        if (string.IsNullOrEmpty(path)) return;

    //        // Jeśli player nie ma jeszcze źródła — ustaw tylko raz
    //        if (Player.Source == null)
    //            Player.Source = path;

    //        if (!_isPlaying)
    //        {
    //            // Wznów lub rozpocznij odtwarzanie
    //            Player.Play();
    //            PlayButton.Source = "pause_icon.png";
    //            _isPlaying = true;
    //        }
    //        else
    //        {
    //            // Pauza (nie stop!)
    //            Player.Pause();
    //            PlayButton.Source = "play_icon.png";
    //            _isPlaying = false;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Błąd: {ex.Message}");
    //    }
    //}
         private async void Files_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FilesPage(_songs));
        }
        private async void Home_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//MainPage");
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