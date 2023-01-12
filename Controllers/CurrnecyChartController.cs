using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FirstTelegramBot.Controllers
{
    public class CurrnecyChartController
    {
        string _adress;
        HttpWebRequest _request;
        public static List<Rate> rates { get; set; }
        public string Response { get; set; }
        public CurrnecyChartController(string adress)
        {
            _adress = adress;
        }
        public async Task GetCurrencyChart()
        {
            _request = (HttpWebRequest)WebRequest.Create(_adress);
            _request.Method = "Get";

            try
            {
                HttpWebResponse respones = (HttpWebResponse)_request.GetResponse();
                var stream = respones.GetResponseStream();                
                if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                rates = JsonConvert.DeserializeObject<List<Rate>>(Response);
            }
            catch { }                    
        }
    }
}
