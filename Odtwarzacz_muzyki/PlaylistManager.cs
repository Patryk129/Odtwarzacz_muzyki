using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace Odtwarzacz_muzyki
{
    public static class PlaylistManager
    {
        private static readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "playlists.json");

        // Globalna lista playlist
        public static ObservableCollection<Playlist> Playlists { get; } = new();

        // Zapis do pliku
        public static async Task SaveAsync()
        {
            try
            {
                var json = JsonSerializer.Serialize(Playlists);
                await File.WriteAllTextAsync(_filePath, json);
            }
            catch
            {
                // obsługa błędów
            }
        }

        // Odczyt z pliku
        public static async Task LoadAsync()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    var loaded = JsonSerializer.Deserialize<ObservableCollection<Playlist>>(json);

                    if (loaded != null)
                    {
                        Playlists.Clear();
                        foreach (var p in loaded)
                            Playlists.Add(p);
                    }
                }
            }
            catch
            {
                // obsługa błędów
            }
        }
    }
}
