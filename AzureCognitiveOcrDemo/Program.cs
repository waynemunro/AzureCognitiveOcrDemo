using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Runtime.Serialization.Json;
using System.IO;

namespace CSHttpClientSample
{
    static class Program
    {
        static void Main()
        {
            MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "37366095424444c2ae3d99e6b47e49e0");

            // Request parameters
            queryString["language"] = "unk";
            queryString["detectOrientation"] = "true";
            var uri = "https://westcentralus.api.cognitive.microsoft.com/vision/v2.0/ocr?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{'url':'http://www.blueleaf-book-scanning.com/samples/original_test_sample0005.jpg'}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                var stringContent = await response.Content.ReadAsStringAsync();

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Rootobject));

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(stringContent));

                var deserializedRootobject = ser.ReadObject(ms) as Rootobject;

                //Console.Write(stringContent);

                try
                {
                    foreach (var region in deserializedRootobject.regions)
                    {
                        Console.WriteLine($"region {region.boundingBox}\n");

                        foreach (var line in region.lines)
                        {
                            Console.WriteLine("");

                            foreach (var word in line.words)
                            {
                                Console.Write($"{word.text} ");
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine(stringContent);
                    throw;
                }

                
            }
        }
    }


    public class Rootobject
    {
        public string language { get; set; }
        public float textAngle { get; set; }
        public string orientation { get; set; }
        public Region[] regions { get; set; }
    }

    public class Region
    {
        public string boundingBox { get; set; }
        public Line[] lines { get; set; }
    }

    public class Line
    {
        public string boundingBox { get; set; }
        public Word[] words { get; set; }
    }

    public class Word
    {
        public string boundingBox { get; set; }
        public string text { get; set; }
    }

}