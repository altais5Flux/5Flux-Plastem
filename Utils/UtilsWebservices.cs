using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebservicesSage.Utils
{
    public static class UtilsWebservices
    {
        public static string SendData(string url, string xmlData)
        {
            Console.WriteLine(url);
            //System.Threading.Thread.Sleep(2000);
            string xml = xmlData;
            xml = xml.Replace(@"&", "mpa-_-");

            //Console.WriteLine(xml);

            var request = (HttpWebRequest)WebRequest.Create(url);
            var data = Encoding.UTF8.GetBytes("xml=" + xml);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return null;
            }

        }

        public static string SendDataNoParse(string url, string xmlData)
        {
            Console.WriteLine(url);
            //System.Threading.Thread.Sleep(2000);
            string xml = xmlData;
            //Console.WriteLine(xml);

            var request = (HttpWebRequest)WebRequest.Create(url);
            var data = Encoding.UTF8.GetBytes("xml=" + xml);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            if (xml.Equals("fixCart"))
            {
                request.Timeout = 300000;
            }
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return responseString;
        }
    }
}
