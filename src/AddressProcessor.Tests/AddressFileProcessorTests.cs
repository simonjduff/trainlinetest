using AddressProcessing.Address;
using AddressProcessing.Address.v1;
using Xunit;

namespace AddressProcessing.Tests
{
    public class AddressFileProcessorTests
    {
        private FakeMailShotService _fakeMailShotService;
        private const string TestInputFile = @"test_data\contacts.csv";

        public AddressFileProcessorTests()
        {
            _fakeMailShotService = new FakeMailShotService();
        }

        [Fact]
        public void Should_send_mail_using_mailshot_service()
        {
            var processor = new AddressFileProcessor(_fakeMailShotService);
            processor.Process(TestInputFile);

            Assert.Equal(229, _fakeMailShotService.Counter);
        }

        internal class FakeMailShotService : IMailShot
        {
            internal int Counter { get; private set; }

            public void SendMailShot(string name, string address)
            {
                Counter++;
            }
        }
    }
}