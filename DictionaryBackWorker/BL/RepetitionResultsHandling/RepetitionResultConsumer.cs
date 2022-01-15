using DictionaryBack.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DictionaryBackWorker
{
    public class RepetitionResultConsumer : IConsumer<RepetitionEndedMessage>
    {
        private ILogger<RepetitionResultConsumer> _logger;

        public RepetitionResultConsumer(ILogger<RepetitionResultConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RepetitionEndedMessage> context)
        {
            _logger.LogInformation("Received Text: {Text}", context.Message.WordsRepetitionResults.Length);

            return Task.CompletedTask;
        }
    }
}