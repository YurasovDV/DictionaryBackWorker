using DictionaryBack.Messages;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
            _logger.LogInformation("Repetition ended");
            var results = context.Message.WordsRepetitionResults.GroupBy(r => r.RepetitionStatus);

            LogList(results, RepetitionStatus.Success, "Successfully trained: {Term}");
            LogList(results,  RepetitionStatus.FailOnce, "Made one mistake for: {Term}");
            LogList(results,  RepetitionStatus.FailedMultipleTimes, "Made several mistakes for: {Term}");


            return Task.CompletedTask;

            void LogList(IEnumerable<IGrouping<RepetitionStatus, WordRepetitionResult>> results, RepetitionStatus statusToPrint, string message)
            {
                var groupToPrint = results.FirstOrDefault(g => g.Key == statusToPrint);
                if (groupToPrint != null)
                {
                    foreach (var result in groupToPrint)
                    {
                        if (statusToPrint == RepetitionStatus.Success)
                        {
                            _logger.LogInformation(message, result.Term);
                        }
                        else
                        {
                            _logger.LogWarning(message, result.Term);
                        }
                    }
                }
            }
        }
    }
}