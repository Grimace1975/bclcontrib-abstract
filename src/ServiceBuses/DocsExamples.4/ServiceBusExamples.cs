using System.Abstract;
namespace Example
{
    public class MyMessage
    {
        public string Value { get; set; }
    }

    public class MyMessageHandler : IServiceMessageHandler<MyMessage>
    {
        public void Handle(MyMessage message)
        {
            // DO WORK
        }
    }

    class ServiceBusExamples
    {
        private void Main(IServiceBus bus)
        {
            // sending from an instance
            ServiceBus.Send(new MyMessage { Value = "Value" });
            // sending from a builder
            ServiceBus.Send<MyMessage>(x => { x.Value = "Value"; });

            // sending from an instance
            bus.Send(new MyMessage { Value = "Value" });
            // sending from a builder
            bus.Send<MyMessage>(x => { x.Value = "Value"; });
        }
    }
}
