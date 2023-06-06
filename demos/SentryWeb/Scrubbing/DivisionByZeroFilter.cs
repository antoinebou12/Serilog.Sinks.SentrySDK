using Serilog.Events;
using Serilog.Core;

namespace SentryWeb.Scrubbing
{
    public class DivisionByZeroFilter : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            // Replace "zero" with "##-HERO-##" in the log event message
            var filteredMessage = logEvent.MessageTemplate.Text.Replace("zero", "##-HERO-##");
            var messageProperty = propertyFactory.CreateProperty("Message", filteredMessage);

            // Update the log event with the filtered message property
            logEvent.AddOrUpdateProperty(messageProperty);
        }
    }
}
