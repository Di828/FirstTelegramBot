global using NbrbAPI.Models;
global using FirstTelegramBot.Controllers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace FirstTelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {            
            var botClient = new TelegramBotClient("5806467283:AAHxKsCBERwPSNaQ-KXy_Gnrzkoi1kxXT5E");
            var chat = new BotChat();
            var currencyChart = new CurrnecyChartController(@"https://www.nbrb.by/api/exrates/rates?periodicity=0");
            currencyChart.GetCurrencyChart();

            GetBotInfo(botClient);
            chat.BotChating(botClient);

            Console.ReadLine();            
        }                

        public static async void GetBotInfo(TelegramBotClient botClient)
        {            
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        }
    }
}