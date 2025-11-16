using System.Collections.ObjectModel;
using System.Text.Json;

namespace Odtwarzacz_muzyki;

public partial class FilesPage : ContentPage
{
    private readonly ObservableCollection<Song> _songs;
    private readonly string _songsFilePath = Path.Combine(FileSystem.AppDataDirectory, "songs.json");
    public FilesPage(ObservableCollection<Song> songs)
    {
        InitializeComponent();
        _songs = songs; 
        SongsCollectionView.ItemsSource = _songs;
        _ = LoadSongsAsync();
    }

    private async void Add_Clicked(object sender, EventArgs e)
    {
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
                    string author = await DisplayPromptAsync("Autor", $"Podaj autora utworu '{file.FileName}':");
                    var image = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = "Wybierz okładkę",
                        FileTypes = FilePickerFileType.Images
                    });

                    string imagePath;

                    if (image != null)
                    {
                        imagePath = image.FullPath;
                    }
                    else
                    {
                        imagePath = "default_playlist.png"; 
                    }


                    _songs.Add(new Song
                    {
                        Title = System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                        Path = file.FullPath,
                        Author = author,
                        ImagePath = imagePath
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