using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BotApi.Domain.CsvModels;
using BotApi.Domain.Notifications;
using BotApi.Infrastructure;
using CsvHelper;
using Flurl.Http;
using Microsoft.Extensions.Configuration;

namespace BotApi.Domain.Entities
{
    public class Command
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public IConfiguration Configuration { get; set; }
        public INotificationContext NotificationContext { get; set; }
        public IMessageBroker MessageBroker { get; set; }

        public Command(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public async Task ApplyAction()
        {
            var endpoint = Configuration[$"AvailableCommands:{Name}:Endpoint"];

            if (string.IsNullOrEmpty(endpoint))
            {
                NotificationContext.AddNotification((int)HttpStatusCode.BadRequest, "Invalid command, try to fix the syntax");
                return;
            }

            var path = Configuration[$"AvailableCommands:{Name}:Path"];

            var quotes = await GetCsvAsync(endpoint, path);

            MessageBroker.PublishInQueue(BuildMessage(quotes.FirstOrDefault()?.Close));
        }

        public async Task<List<CsvModel>> GetCsvAsync(string endpoint, string path)
        {
            try
            {
                endpoint = endpoint + Value.ToLower() + path;

                var response = await endpoint.WithHeader("Accept", "text/plain").GetAsync();
                var csvResponseMessage = await response.GetStringAsync();

                using var reader = new StreamReader(csvResponseMessage);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var records = csv.GetRecords<CsvModel>();

                return records.ToList();
            }
            catch
            {
                return new List<CsvModel>();
            }
        }

        public string BuildMessage(string close)
        {
            var message = string.IsNullOrEmpty(close) ?
                "Sorry, we're facing troubles in quotation services":
                Value.ToUpper() + " quote is $" + close + "per share";

            return message;
        }
    }
}
