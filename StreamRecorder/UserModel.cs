using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace StreamRecorder
{
  class UserModel : INotifyPropertyChanged
  {
    //https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-5.0
    //https://tocsworld.wordpress.com/2014/04/14/windowsforms%E3%81%A7%E3%81%AE%E5%8D%98%E9%A0%85%E3%83%87%E3%83%BC%E3%82%BF%E3%83%90%E3%82%A4%E3%83%B3%E3%83%89%E3%81%BE%E3%82%8F%E3%82%8A/
    private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private string _songTitle;

    public int id { get; set; }
    public string station { get; set; }
    public string url { get; set; }
    public string bitrate { get; set; }
    public string songTitle {
      set
      {
        _songTitle = value;
        NotifyPropertyChanged();
      }
      get { return _songTitle; }
    }
    public string audioType { get; set; }
    public string isRipping { get; set; }
    public Icy.StreamRecorder recorder { get; set; }

    public void showTitle(object sender, Icy.SongTitleEventArgs e)
    {
      //返されたデータを取得し表示
      this.songTitle = e.SongTitle;
    }
  }
}

