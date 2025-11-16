using System.Collections.ObjectModel;
using Odtwarzacz_muzyki;

namespace Odtwarzacz_muzyki;

public partial class PlaylistPage : ContentPage
{
    private Playlist _selectedPlaylist;
    private ObservableCollection<Song> _allSongs;

    public PlaylistPage(ObservableCollection<Song> allSongs)
    {
        InitializeComponent();
        _allSongs = allSongs;
        PlaylistList.ItemsSource = PlaylistManager.Playlists;

        _ = PlaylistManager.LoadAsync();
    }

    private async void AddPlaylist_Clicked(object sender, EventArgs e)
    {
        string name = await DisplayPromptAsync("Nowa playlista", "Podaj nazwê:");
        if (string.IsNullOrWhiteSpace(name)) return;

        FileResult imageFile = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Wybierz obraz playlisty",
            FileTypes = FilePickerFileType.Images
        });

        string imagePath;

        if (imageFile != null && !string.IsNullOrEmpty(imageFile.FullPath))
        {
            imagePath = imageFile.FullPath;
        }
        else
        {
            imagePath = "default_playlist.png";
        }


        var newPlaylist = new Playlist
        {
            Name = name,
            ImagePath = imagePath
        };

        PlaylistManager.Playlists.Add(newPlaylist);
        await PlaylistManager.SaveAsync();
    }

    private void OpenPlaylist_Clicked(object sender, EventArgs e)
    {
        _selectedPlaylist = null;
        if (sender is Button clickedButton && clickedButton.BindingContext is Playlist playlist)
        {
            _selectedPlaylist = playlist;
        }
        if (_selectedPlaylist == null) return;

        PlaylistTitle.Text = _selectedPlaylist.Name;
        SongsList.ItemsSource = _selectedPlaylist.Songs;

        PlaylistList.IsVisible = false;
        PlaylistDetails.IsVisible = true;
    }

    private void Back_Clicked(object sender, EventArgs e)
    {
        PlaylistDetails.IsVisible = false;
        PlaylistList.IsVisible = true;
        _selectedPlaylist = null;
    }

    private async void AddSong_Clicked(object sender, EventArgs e)
    {
        if (_selectedPlaylist == null || _allSongs.Count == 0) return;

        string[] songTitles = _allSongs.Select(s => s.Title).ToArray();

        string selectedTitle = await DisplayActionSheet(
            "Wybierz piosenkê do dodania",
            "Anuluj",
            null,
            songTitles
        );

        if (string.IsNullOrEmpty(selectedTitle) || selectedTitle == "Anuluj") return;

        Song songToAdd = _allSongs.FirstOrDefault(s => s.Title == selectedTitle);
        if (songToAdd == null) return;

        if (!_selectedPlaylist.Songs.Contains(songToAdd))
        {
            _selectedPlaylist.Songs.Add(songToAdd);
            await PlaylistManager.SaveAsync();
        }

        SongsList.ItemsSource = null;
        SongsList.ItemsSource = _selectedPlaylist.Songs;
    }

    private async void PlaySong_Clicked(object sender, EventArgs e)
    {
        var song = (sender as Button)?.BindingContext as Song;
        if (song == null || _selectedPlaylist == null) return;

        var playlistSongs = new ObservableCollection<Song>(_selectedPlaylist.Songs);

        var songPage = new SongPage(playlistSongs);
        songPage.SetCurrentSong(song);

        await Navigation.PushAsync(songPage);
    }

    private async void DeleteSong_Clicked(object sender, EventArgs e)
    {
        var song = (sender as Button)?.BindingContext as Song;
        if (song == null || _selectedPlaylist == null) return;

        bool confirm = await DisplayAlert("Usuñ", $"Usun¹æ '{song.Title}'?", "Tak", "Nie");
        if (!confirm) return;

        _selectedPlaylist.Songs.Remove(song);
        await PlaylistManager.SaveAsync();

        SongsList.ItemsSource = null;
        SongsList.ItemsSource = _selectedPlaylist.Songs;
    }

    private async void Home_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage(_allSongs));
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
