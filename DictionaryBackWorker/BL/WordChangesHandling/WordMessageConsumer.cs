using System.Threading.Tasks;
using DictionaryBack.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DictionaryBackWorker
{
    public class WordMessageConsumer : IConsumer<WordMessage>
    {
        private ILogger<WordMessageConsumer> _logger;

        public WordMessageConsumer(ILogger<WordMessageConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<WordMessage> context)
        {
            _logger.LogInformation("Received message: {Term} {IsDeleted}", context.Message.Term, context.Message.IsDeleted);
            throw new System.Exception();
        }
    }
}
