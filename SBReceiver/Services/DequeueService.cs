using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using SBShared.Models;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SBReceiver.Services
{
    public class DequeueService : IDequeueService
    {
        private readonly IQueueClient _queueClient;

        public DequeueService(IConfiguration config)
        {
            _queueClient = new QueueClient(config.GetConnectionString("AzureServiceBus"), config.GetValue<string>("QueueName"));
        }

        public async Task Run()
        {
            GreetingMessage();

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1, // process only 1 msg at a time
                AutoComplete = false // not mark as complete
            };

            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.ReadLine();

            await _queueClient.CloseAsync();
        }

        private static void GreetingMessage()
        {
            var description = "This is an app to illustrate how microservices work with Azure Service Bus.";
            Console.WriteLine("Azure Service Bus - Receiver");
            Console.WriteLine(description);
            Console.WriteLine("{0} {1}", Regex.Replace(description, ".", "="), Environment.NewLine);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var jsonString = Encoding.UTF8.GetString(message.Body);
            var person = JsonSerializer.Deserialize<PersonModel>(jsonString);
            Console.WriteLine($"Person Received: {person?.FirstName} {person?.LastName}");

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine($"Message handler exception: {args.Exception}");
            return Task.CompletedTask;
        }
    }
}
