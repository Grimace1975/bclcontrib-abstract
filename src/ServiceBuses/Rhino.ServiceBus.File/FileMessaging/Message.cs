using System.IO;
namespace Rhino.ServiceBus.FileMessaging
{
    public class Message
    {
        public string Label { get; set; }
        public byte[] Extension { get; set; }
        public int AppSpecific { get; set; }
        public MessageQueue ResponseQueue { get; set; }
        public Stream BodyStream { get; set; }
        public string Id { get; set; }
        public string CorrelationId { get; set; }
        public string Body { get; set; }
        public int BodyType { get; set; }
    }
}
