/*
 * Author : RTU(keroroxzz)
 * Last modify : 2023/2/10
 * 
 * Request the latest version on the Github to check update.
 */

using System;
using System.Net;
using System.Diagnostics;

namespace DRnamespace
{
    public class UpdateManager
    {
        private HttpWebRequest wr;
        private string myVersion = "1.3.2";
        private string latestVersion = "";

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
            if (latestVersion.Length > 0)
                return latestVersion;

            Trace.WriteLine("Requesting the latest verstion on Github...");
            string version = "Failed to get the version.";
            try
            {
                HttpWebResponse resp = (HttpWebResponse)wr.GetResponse();

                var encoding = resp.ContentEncoding;
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();

                    string tag_name = "\"tag_name\":";
                    int tag = responseText.IndexOf(tag_name) + tag_name.Length,
                        ver_beg = responseText.IndexOf("\"", tag) + 1,
                        ver_end = responseText.IndexOf("\"", ver_beg);
                    version = responseText.Substring(ver_beg, ver_end - ver_beg);
                }

                Trace.WriteLine("The latest version is: " + version);

                // Must assign the version to a variable so that this function becomes kinda sync
                latestVersion = version;

                resp.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Trace.WriteLine(ex.Message);
            }

            return version;
        }

        public string LatestVersion()
        {
            return RequestLatesVersion();
        }

        public bool IsAnyUpdate()
        {
            return RequestLatesVersion() != myVersion;
        }
    }
}
