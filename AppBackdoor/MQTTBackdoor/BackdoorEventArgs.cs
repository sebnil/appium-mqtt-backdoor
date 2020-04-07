using System;
using System.Linq;

namespace MQTTBackdoor
{
    public class BackdoorEventArgs : EventArgs
    {
        public string Topic { get; private set; }

        public string Subtopic { get; private set; }

        public string Payload { get; private set; }

        public BackdoorEventArgs(string topic, string payload)
        {
            Topic = topic;
            var subTopics = topic.Split('/');
            var subTopicsWithoutBaseTopic = string.Join("/", subTopics.Skip(1));
            Subtopic = subTopicsWithoutBaseTopic;
            Payload = payload;
        }
    }
}
