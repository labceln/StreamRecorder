using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace StreamRecorder
{
  
  public partial class Form1 : Form
  {
    const string _formTitle = "StreamRecorder";
    private List<UserModel> _tasks;
    private string _saveFolder="";

    public Form1()
    {
      
      InitializeComponent();

      _tasks = new List<UserModel>();
      userModelBindingSource.DataSource = _tasks;
      _saveFolder = Properties.Settings.Default.LastSelectedFolder;
      this.Text = _formTitle + " 保存先" + "<" + _saveFolder + ">";
    }
    
    private async void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

      DataGridView dgv = (DataGridView)sender;

      //列がisRippingではないか、押したのがヘッダー
      if (!(dgv.Columns[e.ColumnIndex].Name == "isRipping" && e.RowIndex >= 0))
        return;

      if (string.IsNullOrEmpty(_saveFolder))
      {
        MessageBox.Show("保存先が空です");
        return;
      }

      //行から対応するデータを取得
      DataGridViewRow row = dgv.Rows[e.RowIndex];
      UserModel u = row.DataBoundItem as UserModel;

      #region ripOrStop
      //ボタン表示がRIPのときリッピングを行う
      if (u.isRipping == "RIP")
      {
        //ボタンの表示をSTOPに変える
        u.isRipping = "STOP";

        //URLからStreamRecorderの準備を行う
        try
        {
          u.recorder = new Icy.StreamRecorder();
          u.recorder.prepare(u.url, "/");
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex.Message);
          //失敗したらボタン表示を元に戻す
          u.isRipping = "RIP";
          return;
        }

        //放送局名の情報の列挙
        foreach (string key in u.recorder._stationHeaders.Keys)
        {
          Debug.WriteLine(key + " : " + u.recorder._stationHeaders[key]);
        }
        //各欄への情報入力
        u.station = u.recorder._stationHeaders["icy-name"];
        u.bitrate = u.recorder._stationHeaders["icy-br"];
        u.audioType = u.recorder._stationHeaders["Content-Type"];

        //曲名が変わったときのイベントハンドラ
        u.recorder.songTitleEvent += new Icy.StreamRecorder.ChangeSongEventHandler(u.showTitle);

        try
        {
          //保存を行う
          await Task.Run(() => u.recorder.record(_saveFolder));
        }
        catch (Exception ex)
        {
          Debug.WriteLine(ex.Message);
          //失敗したらボタン表示を元に戻す
          u.isRipping = "RIP";
          return;
        }
      }

      else if (u.isRipping == "STOP")
      {
        u.recorder.stop();
        u.isRipping = "RIP";
      }
      else
      {
        Debug.WriteLine("Button Text is empty");
        MessageBox.Show("Button Text is empty");
        return;
      }
      #endregion
    }


    private void fileToolStripMenuItem_Click(object sender, EventArgs e)
    {
      string m3uFileOpen()
      {
        #region m3uFileOpen
        //OpenFileDialogクラスのインスタンスを作成
        OpenFileDialog ofd = new OpenFileDialog();

        //[ファイルの種類]に表示される選択肢を指定する
        //指定しないとすべてのファイルが表示される
        ofd.Filter = "m3u8ファイル(*.m3u8)|*.m3u8|すべてのファイル(*.*)|*.*";
        //[ファイルの種類]ではじめに選択されるものを指定する
        //2番目の「すべてのファイル」が選択されているようにする
        ofd.FilterIndex = 0;
        //  タイトルを設定する
        ofd.Title = "開くファイルを選択してください";
        //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
        ofd.RestoreDirectory = true;

        //ダイアログを表示する
        if (ofd.ShowDialog() != DialogResult.OK)
          return null;

        return ofd.FileName;
        #endregion
      }

      void m3uFile_To_DataRow(List<string> list)
      {
        #region m3uFile_To_DataRow

        //すべて行で保存を止める
        foreach (DataGridViewRow row in dataGridView1.Rows)
        {
          var u=row.DataBoundItem as UserModel;
          u.recorder?.stop();
        }
        userModelBindingSource.Clear();

        try
        {
          for (int i = 0; i < list.Count / 2; i++)
          {

            //Console.WriteLine(list[2*i]);//1行目で放送局名
            //Console.WriteLine(list[2*i+1]);//2行目でURL

            //:-?               :と、整数の符号があれば
            //\d+               整数が必要
            //,([^\r?\n]+)      ,から改行までを抽出
            string pattern = @"#EXTINF:-?\d+,([^\r?\n]+)";
            
            foreach (Match m in Regex.Matches(list[2 * i], pattern))
            {
              Debug.WriteLine("station: {0}", Regex.Match(list[2 * i], pattern).Groups[1].Value);
            }
            string splitted = Regex.Match(list[2 * i], pattern).Groups[1].Value;

            userModelBindingSource.Add(new UserModel { id = i, station = splitted, url = list[2 * i + 1],isRipping="RIP" });
          }
        }
        catch (Exception ex)
        {
          MessageBox.Show("error on parsing m3u\n" + ex.Message);
        }
        #endregion
      }

      string fileName = m3uFileOpen();

      if (string.IsNullOrEmpty(fileName))
        return;

      //OKボタンがクリックされたとき、選択されたファイル名を表示する
      Console.WriteLine(fileName);

      string line="";
      var lines = new List<string>();

      using (System.IO.StreamReader sr = new System.IO.StreamReader(
         fileName, Encoding.UTF8))
      {
        //1行目は"#EXTM3U"のはず
        line = sr.ReadLine();
        if (line != "#EXTM3U") {
          MessageBox.Show("missing #EXTM3U m3u File.");
          return;
        }

        while ((line = sr.ReadLine()) != null)
        {
          lines.Add(line);
        }
      }

      //M3Uファイルの2行を1レコードに変換
      m3uFile_To_DataRow(lines);
    }

    private void saveFolderToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("現在の保存先 : " + (string.IsNullOrEmpty(_saveFolder) ? "なし" : _saveFolder)
        +"\n"+"現在の保存中のストリーミングがすべて停止されます。");

      //FolderBrowserDialogクラスのインスタンスを作成
      FolderBrowserDialog fbd = new FolderBrowserDialog();

      //上部に表示する説明テキストを指定する
      fbd.Description = "フォルダを指定してください。";

      //ルートフォルダを指定する
      //デフォルトでDesktop
      fbd.RootFolder = Environment.SpecialFolder.Desktop;

      //最初に選択するフォルダを指定する
      fbd.SelectedPath = Properties.Settings.Default.LastSelectedFolder;
      //ユーザーが新しいフォルダを作成できるようにする
      //デフォルトでTrue
      fbd.ShowNewFolderButton = true;

      //ダイアログを表示する
      if (fbd.ShowDialog(this) != DialogResult.OK)
        return;

      //すべて行で保存を止める
      foreach (DataGridViewRow row in dataGridView1.Rows)
      {
        var u = row.DataBoundItem as UserModel;
        u.recorder?.stop();
      }

      //選択されたフォルダを表示する
      Console.WriteLine(fbd.SelectedPath);

      _saveFolder = fbd.SelectedPath;
      Properties.Settings.Default.LastSelectedFolder=fbd.SelectedPath;
      Properties.Settings.Default.Save();
      this.Text = _formTitle+ " 保存先" +"<"+_saveFolder+ ">";
    }
  }
}
