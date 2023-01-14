using System.IO;
using System.Net;
using System.Text;

namespace XIVChatToQQ.Helper;
public class HttpHelper
{
  public static string Request(string data, string url)
  {
    return Request(Encoding.GetEncoding("UTF-8").GetBytes(data), url);
  }

  public static string Request(byte[] data, string url)
  {
    string result = string.Empty;

    //创建一个客户端的Http请求实例
    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
    request.ContentType = "application/json";
    request.Method = "POST";
    request.ContentLength = data.Length;
    Stream requestStream = request.GetRequestStream();
    requestStream.Write(data, 0, data.Length);
    requestStream.Close();

    //获取当前Http请求的响应实例
    HttpWebResponse response = request.GetResponse() as HttpWebResponse;
    Stream responseStream = response.GetResponseStream();
    using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
    {
      result = reader.ReadToEnd();
    }
    responseStream.Close();

    return result;
  }
}