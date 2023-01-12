using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Threading;
using FirstTelegramBot.Models;

namespace FirstTelegramBot
{
    internal class BotChat
    {
        private Dictionary<string, List<string>> possibleCurrencies = new Dictionary<string, List<string>> {
            {"USD", new List<string>{"usd","доллар","доллара","$","бакс","бакса" } },
            {"EUR", new List<string>{"euro","eur","евро" } }
        };
        private TicTacToeController _ticTakController;
        private string _condition = "default";
        private ToDoListController _toDoController = new ToDoListController("https://localhost:7240/api/ToDoList");
        public async void BotChating(TelegramBotClient botClient)
        {
            using CancellationTokenSource cts = new();
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };
            botClient.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync
            //receiverOptions: receiverOptions,
            //cancellationToken: cts.Token
            );
            Console.ReadLine();
            cts.Cancel();
        }
        private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message is not { } message)
                return;
            if (message.Text is not { } messageText)
                return;
            var chatId = message.Chat.Id;

            Console.WriteLine($"Receave a message {message} in chat {chatId}");

            string answerMessage = FormAnswerMessage(messageText);  
            
            await botClient.SendTextMessageAsync(chatId, answerMessage);
        }
        private string FormAnswerMessage(string messageText)
        {
            var answerMessage = "";
            if (messageText.ToLower() == "выход" && _condition != "default")
            {
                _condition = "default";
                answerMessage = "Чем еще займемся?";
                return answerMessage;
            }
            while (_condition == "tictak")
            {
                answerMessage = _ticTakController.OpponentsTurn(messageText.ToLower());
                if (_ticTakController.GameOver)
                {
                    _condition = "default";
                    answerMessage += " Чем еще займемся?";
                }
                return answerMessage;
            }
            while (_condition == "todos")
            {
                answerMessage = _toDoController.WorkWithToDoList(messageText.ToLower()).Result;
                return answerMessage;
            }
            if (messageText.ToLower().Contains("помощь"))
                answerMessage = "Чтобы узнать курсы валют, введите : курс \"валюта\", к примеру курс доллара.\n\r " +
                    "Если хотите услышать шутку про Чака Норриса, введите : чак.\n\r" +
                    "Чтобы сыграть в крестики-нолики введите : игра.\n\r" +
                    "Чтобы увиделть список дел введите : дела";
            else if (messageText.ToLower().Contains("курс"))
            {
                var currencyData = ParseCurrency(messageText.ToLower());
                if (currencyData.IsParsed)
                    answerMessage = FormChartMessage(currencyData.Currency);
                else answerMessage = currencyData.Message;
            }
            else if (messageText.ToLower().Contains("чак"))
                answerMessage = ChakNorrisJoke().Result;
            else if (messageText.ToLower().Contains("игра"))
            {
                _condition = "tictak";
                answerMessage = StartGameMessage();                
            }
            else if (messageText.ToLower().Contains("дел"))
            {
                _condition = "todos";
                answerMessage = _toDoController.Start().Result;                
            }
            else
                answerMessage = "Привет, чтобы узнать о моих возможностях, используйте команду \"помощь\".";
            return answerMessage;
        }                
        private string StartGameMessage() 
        {
            _ticTakController = new TicTacToeController();            
            return _ticTakController.StartGameMessage();
        }
        private async Task<string> ChakNorrisJoke()
        {
            ChackNorrisJokeController _jokeController = new ChackNorrisJokeController("https://api.chucknorris.io/jokes/random");
            await _jokeController.GetJoke();
            return ChackNorrisJokeController.Data.value;
        }
        private string FormChartMessage(string currensy_Abb)
        {
            var result = "";
            try
            {
                var usdChart = CurrnecyChartController.rates.First(x => x.Cur_Abbreviation == currensy_Abb);
                result = $"За {usdChart.Cur_Scale} {usdChart.Cur_Name} просят {usdChart.Cur_OfficialRate}\n\r";
            }
            catch
            {

            }
            return result;
        }
        private CurrencyParseData ParseCurrency(string message)
        {
            var words = message.Split(' ');
            CurrencyParseData result = new CurrencyParseData();
            if (words.Length < 2)
            {                
                result.IsParsed = false;
                result.Message = "укажите валюту, курс которой вы хотите узнать";
                return result;
            }
            foreach (var x in possibleCurrencies)
                if (x.Value.Contains(words[1]))
                {
                    result.IsParsed = true;
                    result.Currency = x.Key;
                    return result;
                }
            result.IsParsed = false;
            result.Message = "извините, валюта не поддерживается";
            return result;
        }
    }
}
