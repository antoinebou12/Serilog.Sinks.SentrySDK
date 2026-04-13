using Moq;

using Sentry;

using Xunit;

namespace Serilog.Sinks.SentrySDK.Tests
{
    public class TransactionServiceTests
    {
        [Fact]
        public void StartChild_DelegatesToTransaction_StartChild()
        {
            // Two-arg StartChild is SpanExtensions; Moq cannot mock extension methods.
            // The extension calls ISpan.StartChild(operation) then sets Description.
            var childSpan = new Mock<ISpan>();
            childSpan.SetupProperty(s => s.Description);

            var transaction = new Mock<ITransactionTracer>();
            transaction.Setup(t => t.StartChild("op")).Returns(childSpan.Object);

            var service = new TransactionService();
            var result = service.StartChild(transaction.Object, "op", "desc");

            Assert.Same(childSpan.Object, result);
            Assert.Equal("desc", result.Description);
            transaction.Verify(t => t.StartChild("op"), Times.Once);
        }
    }
}
