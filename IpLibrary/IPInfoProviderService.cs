using IPLibrary.CustomExceptions;
using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace IPLibrary
{
    public class IPInfoProviderService : IIPInfoProvider
    {
        private readonly IConfiguration config;
        public IPInfoProviderService(IConfiguration config)
        {
            this.config = config;
        }

        public IPDetails GetDetails(string ip)
        {
            try
            {
                /* Get access key 
                 * (IPStackAccessKeys : DefaultAccessKey in appSettings.Development)
                 */
                string accessKey = config.GetSection("IPStackAccessKeys").GetSection("DefaultAccessKey").Value;

                IPDetails iPDetails = new();

                // Example http://api.ipstack.com/46.11.43.239?access_key=dde84e792ec7ffd3cb982df0c1b8cf87&format=1
                string URL = String.Format("http://api.ipstack.com/{0}?access_key={1}&format=1", ip, accessKey);

                HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(IPStackURL);

                // Header
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                // Data response
                HttpResponseMessage response = client.GetAsync(URL).Result;
                if (response.IsSuccessStatusCode)
                {
                    // Successful. Get JSON string and convert to our model
                    var contentStream = response.Content.ReadAsStreamAsync().Result;

                    using var streamReader = new StreamReader(contentStream);
                    using var jsonReader = new JsonTextReader(streamReader);

                    JsonSerializer serializer = new JsonSerializer();

                    try
                    {
                        iPDetails = serializer.Deserialize<IPDetails>(jsonReader);
                    }
                    catch (JsonReaderException)
                    {
                        throw new Exception("An error was encountered during IP request: " + ip);
                    }
                }
                else
                {
                    /* Unsuccessful. Let's assume this is very unusual 
                     * behaviour and treat this as an exception.
                     */
                    throw new Exception("IP: " + ip + " not found!");
                }

                client.Dispose();

                return iPDetails;

            }
            catch (Exception ex)
            {
                throw new IPServiceNotAvailableException(ex.Message);
            }
        }
    }
}
