using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;


namespace DictionaryBackWorker
{
    public class MessageConsumer : IConsumer<Message>
    {
        private ILogger<MessageConsumer> _logger;

        public MessageConsumer(ILogger<MessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.Term);

            return Task.CompletedTask;
        }
    }

    public class Message
    {
        public string Term { get; set; }

        public bool Remove { get; set; }

    }
}
