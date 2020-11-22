using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;


namespace Icy
{

  public class SongTitleEventArgs : EventArgs
  {
	public string SongTitle;
  }

  enum StreamState
  {
	Pass1stStream,
	Check1stMetaData,
	WriteStream,
	CheckMetaData
  }

  class StreamRecorder
  {
	int _metaInt = 0;
	Stream _byteOut = null;
	string _oldMetadataHeader = "";
	int _count = 0;
	StreamState _phase=StreamState.Pass1stStream;
	int _metadataLength = 0;
	bool _noMeta = true;

	List<byte> _metadataBytes = new List<byte>();
	Stream _socketStream = null;
	int _fileCount = 0;
	bool _isRipping = false;


	public Dictionary<string, string> _stationHeaders { get; }
	public delegate void ChangeSongEventHandler(object sender, SongTitleEventArgs e);
	public event ChangeSongEventHandler songTitleEvent=null;

	public StreamRecorder()
	{
	  _stationHeaders = new Dictionary<string, string>();
	}

	public void stop()
    {
	  _isRipping = false;
    }

	public void prepare(string server, string serverPath)
    {
	  _socketStream = prepareGetStream(server, serverPath);

	}

	public void record( string destPath)
	{
	  _isRipping = true;
	  byte[] buffer = new byte[2048]; // receive buffer
	  string date=DateTime.Now.ToString("yyyy-MM-dd-HH\\h-mm\\m-ss\\s");

	  string station = _stationHeaders["icy-name"];
	  var replaceStrings = new string[] { ":", "/", "\\", "<", ">", "|", "?", "*", "\'", "\"" };
	  foreach (var s in replaceStrings)
	  {
		station = station.Replace(s, "");
	  }
	  destPath += "\\" + station + " - " + date + "\\";

	  Directory.CreateDirectory(destPath);

	  try
	  {
		while (_isRipping)
		{
		  int bufLen = _socketStream.Read(buffer, 0, buffer.Length);
		  if (bufLen < 0)
			return;


		  foreach (var b in buffer.Take(bufLen))
		  {
			bool write = false;
            switch (_phase)
            {
			  case StreamState.Pass1stStream:
				write = pass1stStream(b);
				break;
			  case StreamState.Check1stMetaData:
				write = check1stMetadata(b, destPath);
				break;
			  case StreamState.WriteStream:
				write = writeStream(b);
				break;
			  case StreamState.CheckMetaData:
				write=checkMetadata(b, destPath);
				break;
			}
            if (write)
            {
			  _byteOut.WriteByte(b);
			  _count++;
			}
		  }
		}
	  }
	  catch (Exception ex)
	  {
		Debug.WriteLine(ex.Message);
		Debug.WriteLine(ex.StackTrace);
	  }
	  finally
	  {
		if (_byteOut != null)
		  _byteOut.Close();
		if (_socketStream != null)
		  _socketStream.Close();
		_isRipping = false;
	  }
	}

	/// <summary>
	/// Streamデータの最初の読み込みだが書き込まず捨てる.
	/// 1ブロックの読み込みが終わったら、メタデータの長さを得る。メタデータが0であれば
	/// 以後メタデータがない放送局としてフラグを立てる。
	/// </summary>
	private bool pass1stStream(byte b)
    {
	  if (_count++ < _metaInt) return false;
      else 
      {
		_phase = StreamState.Check1stMetaData;
		_count = 0;
		_metadataLength = Convert.ToInt32(b)*16;
		if (_metadataLength > 0) _noMeta = false;
		return false;
      }
    }
	/// <summary>
	/// メタデータがある放送局か否かでファイルを曲名で分割するか否か対応を分ける。
	/// </summary>
	private bool check1stMetadata(byte b,string destPath)
    {
	  if (_noMeta)
      {
        if (_metadataLength > 0)
        {
		  Exception e = new Exception("There is MetadataHeader.");
		  throw e;
        }
		_phase = StreamState.WriteStream;
		string ext = getExt(_stationHeaders["Content-Type"]);
		_byteOut = createNewFile(destPath, _stationHeaders["icy-name"], ext);

		SongTitleEventArgs ex = new SongTitleEventArgs();
		ex.SongTitle = _stationHeaders["icy-name"];
		songTitleEvent(this ,ex);
		
		return true;
	  }
	  return readMetadata_CheckNewSong(b,destPath);

	}

	/// <summary>
	/// Streamデータの読み込み.1ブロックの読み込みが終わったら、メタデータの長さを得る
	/// </summary>
	private bool writeStream(byte b)
    {
	  if (_count < _metaInt)
      {
		return true;
      }
	  else
	  {
		_phase = StreamState.CheckMetaData;
		_count = 0;
		_metadataLength = Convert.ToInt32(b) * 16;
		_byteOut.Flush();
		return false;
	  }
	}

	/// <summary>
	/// Metadataを読み取って以前と同じ曲名であれば、新たなファイルを作成する.
	/// </summary>
	private bool readMetadata_CheckNewSong(byte b, string destPath)
    {
	  if (_metadataLength > 0)
	  {
		_metadataBytes.Add(b);
		_metadataLength--;
		return false;
	  }
	  else
	  {
		_phase = StreamState.WriteStream;
		var metadataHeader = Encoding.UTF8.GetString(_metadataBytes.ToArray());

		if (string.IsNullOrEmpty(metadataHeader) )//(1)同じ曲で_metadataLengthが0(_metadataBytesがnull)の場合
		{
		  return true;
		}
		if (metadataHeader.Equals(_oldMetadataHeader))//(2)同じ曲名でも_metadataLength>0の場合もたまにある
		{
		  _metadataBytes.Clear();//次にメタデータがあった場合のため空にする
		  return true;
		}

		if (_byteOut != null)//(3)違う曲名の場合
		{
		  _byteOut.Flush();
		  _byteOut.Close();
		}

		//(3)違う曲名の場合と(4)check1stmetadataの場合
		string ext = getExt(_stationHeaders["Content-Type"]);
		_byteOut = createNewFile(destPath, getFileName(metadataHeader), ext);

		SongTitleEventArgs ex = new SongTitleEventArgs();
		ex.SongTitle = getFileName(metadataHeader);
		songTitleEvent(this, ex);
		
		_oldMetadataHeader = metadataHeader;
		_metadataBytes.Clear();
		return true;
	  }
	}

	/// <summary>
	/// Metadataのあるなしでファイルを分割するか決める.
	/// </summary>
	private bool checkMetadata(byte b, string destPath)
	{
	  if (_noMeta)
	  {
		if (_metadataLength > 0)
		{
		  Exception e = new Exception("There is MetadataHeader.");
		  throw e;
		}
		_phase = StreamState.WriteStream;
		return true;
	  }
	  return readMetadata_CheckNewSong(b, destPath);
	}

	/// <summary>
	/// contentTypeからmp3かaacか判別して返す.
	/// </summary>
	private string getExt(string contentType)
	{
	  if (contentType.Contains("mpeg"))
	  {
		return "mp3";
	  }
	  else if (contentType.Contains("aac"))
	  {
		return "aac";
	  }
	  else
	  {
		Exception e=new Exception("Unknown Content-Type");
		throw e;
	  }
	}


	/// <summary>
	/// WEBからデータをやり取りする準備.
	/// </summary>
	/// <param name="server">放送局のURL</param>
	/// <param name="serverPath">現在のところ'/'で固定</param>
	/// <returns>an output stream on the file</returns>
	private Stream prepareGetStream(string server, string serverPath)
	{
	  var request = (HttpWebRequest)WebRequest.Create(server);
	  HttpWebResponse response = null; // web response

	  // clear old request header and build own header to receive ICY-metadata
	  request.Headers.Clear();
	  request.Headers.Add("GET", serverPath + " HTTP/1.0");
	  request.Headers.Add("Icy-MetaData", "1"); // needed to receive metadata informations
	  request.UserAgent = "WinampMPEG/5.09";

	  // execute request
	  try
	  {
		response = (HttpWebResponse)request.GetResponse();
	  }
	  catch (Exception ex)
	  {
		Debug.WriteLine(ex.Message);
		Debug.WriteLine("prepare failed");
		return null;
	  }

	  _stationHeaders.Clear();
	  foreach (string key in response.Headers.Keys)
	  {
		Debug.WriteLine(key + " : " + response.Headers.Get(key));
		_stationHeaders.Add(key, response.Headers.Get(key));
	  }

	  // read blocksize to find metadata header
	  _metaInt = Convert.ToInt32(response.GetResponseHeader("icy-metaint"));
	  if (_metaInt == 0)
	  {
		_metaInt = int.Parse(_stationHeaders["icy-metaint"]);
	  }

	  return response.GetResponseStream();
	}

	/// <summary>
	///メタデータからファイル名を決める
	/// </summary>
	private string getFileName(string metadataHeader)
	{
	  string[] musicInfos = metadataHeader.Split(';');
	  var d = new System.Collections.Specialized.OrderedDictionary();
	  foreach (var Info in musicInfos)
	  {
		int j = Info.IndexOf("=");
		if (j < 0)
		{
		  d[Info] = null;
		  Debug.WriteLine("[metadataHeaderKey,Value]:" + Info + ",null");
		}
		else
		{
		  j++;//"="の分１文字分進ませる
		  d.Add(Info.Substring(0, j), Info.Substring(j));
		  Debug.WriteLine("[metadataHeaderKey,Value]:" + Info.Substring(0, j) + "," + Info.Substring(j));
		}
	  }

	  var fileName = (d.Contains("StreamTitle") ? (string)d["StreamTitle"] : (string)d[0]);

	  return fileName;
	}

	/// <summary>
	///ファイル名からファイルを作成
	/// </summary>
	private Stream createNewFile(String destPath, String filename, string ext)
	{
	  // replace characters, that are not allowed in filenames. (quick and dirrrrrty ;) )
	  var replaceStrings = new string[] { ":", "/", "\\", "<", ">", "|", "?", "*", "\'", "\"" };
	  foreach (var s in replaceStrings)
	  {
		filename = filename.Replace(s, "");
	  }

	  Debug.WriteLine("[filename]:" + filename);
	  try
	  {
		filename = Convert.ToString(_fileCount++) + ")" + filename;
		// create directory, if it doesn't exist
		if (!Directory.Exists(destPath))
		  Directory.CreateDirectory(destPath);

		// create new file
		if (!File.Exists(destPath + filename + "." + ext))
		{
		  return File.Create(destPath + filename + "." + ext);
		}
		else // if file already exists, don't overwrite it. Instead, create a new file named <filename>(i).mp3
		{
		  for (int i = 1; ; i++)
		  {
			if (!File.Exists(destPath + filename + "(" + i + ")" + "." + ext))
			{
			  return File.Create(destPath + filename + "(" + i + ")" + "." + ext);
			}
		  }
		}
	  }
	  catch (IOException)
	  {
		return null;
	  }
	}
  }
}
