using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odtwarzacz_muzyki
{
    public class Playlist
    {
        public string Name { get; set; }
        public ObservableCollection<Song> Songs { get; set; } = new();
        public string ImagePath { get; set; } = "deafult_playlist.png";
    }

}
