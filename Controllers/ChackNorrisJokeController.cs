using FirstTelegramBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FirstTelegramBot.Controllers
{
    public class ChackNorrisJokeController
    {
        string _adress;
        HttpWebRequest _request;
        public static ChackNorrisJokeData Data { get; set; }
        public string Response { get; set; }
        public ChackNorrisJokeController(string address)
        {
            _adress = address;
        }
        public async Task GetJoke()
        {
            _request = (HttpWebRequest)WebRequest.Create(_adress);
            _request.Method = "Get";            
            try
            {
                HttpWebResponse respones = (HttpWebResponse)_request.GetResponse();
                var stream = respones.GetResponseStream();
                if (stream != null) Response = new StreamReader(stream).ReadToEnd();
                Data = JsonConvert.DeserializeObject<ChackNorrisJokeData>(Response);
            }
            catch { }
            return;
        }
    }
}
