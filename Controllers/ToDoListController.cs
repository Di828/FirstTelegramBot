using FirstTelegramBot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FirstTelegramBot.Controllers
{
    public class ToDoListController
    {
        string _adress;        
        private static readonly HttpClient client = new HttpClient();
        private List<ToDoList> _toDoList { get; set; }        
        public ToDoListController(string adress)
        {
            _adress = adress;
        }
        public async Task GetToDoList()
        {
            var responseString = await client.GetStringAsync(_adress);
            _toDoList = JsonConvert.DeserializeObject<List<ToDoList>>(responseString);
        }
        public async Task<string> Start()
        {
            await GetToDoList();
            var result = ToDoListToAnswer();
            return result;

        }
        public async Task<string> WorkWithToDoList(string inputMessage)
        {
            if (inputMessage.Contains("добавить"))
            {
                try
                {
                    await AddNewToDoItem(inputMessage.Split(' ')[1]);
                    return ToDoListToAnswer();
                }
                catch
                {
                    return "Добавить не может использоваться без элемента который необходимо добавить";
                }
            }
            else if (inputMessage.Contains("удалить"))
            {
                try
                {
                    await DeleteToDoItem(inputMessage.Split(' ')[1]);
                    return ToDoListToAnswer();
                }
                catch
                {
                    return "Необходимо указать id удаляемого объекта";
                }
            }
            return "Команда не известна. Допустимые команды : \"добавить\" \"удалить\" \"выход\"";
        }
        private async Task DeleteToDoItem(string item)
        {
            var deleteMessage = _adress + $"?id={item}";
            var response = await client.DeleteAsync(deleteMessage);
            var responseString = await response.Content.ReadAsStringAsync();
            _toDoList = JsonConvert.DeserializeObject<List<ToDoList>>(responseString);
        }
        private async Task AddNewToDoItem(string item)
        {
            var sendingData = new ToDoList() { Name = item };
            JsonContent content = JsonContent.Create(sendingData);
            
            var response = await client.PostAsync(_adress, content);
            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
            _toDoList = JsonConvert.DeserializeObject<List<ToDoList>>(responseString);
        }
        private string ToDoListToAnswer()
        {
            string result = "Мой список дел:\n\r";
            foreach (var item in _toDoList)
                result = result + item.Id + " " + item.Name + "\n\r";
            result += "Для работы со списком дел используйте команды \"удалить {id}\" и \"добавить {название}\" \"выход\".";
            return result;
        }
    }
}
