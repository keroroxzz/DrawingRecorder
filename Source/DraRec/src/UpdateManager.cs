/*
 * Author : RTU(keroroxzz)
 * Last modify : 2022/04/07
 * 
 * Request the latest version on the Github
 */

using System;
using System.Net;

namespace DRnamespace
{
    public class UpdateManager
    {
        private HttpWebRequest wr;
        public UpdateManager()
        {
            wr = (HttpWebRequest)WebRequest.Create("https://api.github.com/repos/keroroxzz/DrawingRecorder/releases/latest");
            wr.Credentials = CredentialCache.DefaultCredentials;
            wr.ContentType = "application/json; charset=utf-8";
            wr.UserAgent = "response";
            wr.Method = "GET";
        }

        public string RequestLatesVersion()
        {
            try
            {
                HttpWebResponse resp = (HttpWebResponse)wr.GetResponse();

                var encoding = resp.ContentEncoding;
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();

                    string tag_name = "\"tag_name\":";
                    int tag = responseText.IndexOf(tag_name) + tag_name.Length,
                        ver_beg = responseText.IndexOf("\"", tag)+1,
                        ver_end = responseText.IndexOf("\"", ver_beg);
                    Console.WriteLine(responseText.Substring(ver_beg,ver_end-ver_beg));
                }

                resp.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }
}
