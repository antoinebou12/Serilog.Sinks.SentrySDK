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
            var expectedSpan = new Mock<ISpan>().Object;
            var transaction = new Mock<ITransactionTracer>();
            transaction.Setup(t => t.StartChild("op", "desc")).Returns(expectedSpan);

            var service = new TransactionService();
            var result = service.StartChild(transaction.Object, "op", "desc");

            Assert.Same(expectedSpan, result);
            transaction.Verify(t => t.StartChild("op", "desc"), Times.Once);
        }
    }
}
