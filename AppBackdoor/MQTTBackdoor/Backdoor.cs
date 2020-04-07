using System;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;

namespace MQTTBackdoor
{
    public class Backdoor : Singleton<Backdoor>
    {
        public event EventHandler<BackdoorEventArgs> BackdoorEvent;

        private bool initialized = false;
        private string mqttHost;
        private string baseTopic;
        private string clientId;


        
        public async Task Initialize(string mqttHost = "localhost", string baseTopic = "Backdoor", string clientId = "MobileApp")
        {
            this.mqttHost = mqttHost;
            this.baseTopic = baseTopic;
            this.clientId = clientId;

            if (initialized)
            {
                return;
            }

            var configuration = new MqttConfiguration();
            var client = await MqttClient.CreateAsync(this.mqttHost, configuration);
            var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId: this.clientId));

            await client.SubscribeAsync($"{this.baseTopic}/#", MqttQualityOfService.AtMostOnce); // QoS0

            client.MessageStream.Subscribe(msg => HandleReceivedMessage(msg));
            initialized = true;
        }

        private void HandleReceivedMessage(MqttApplicationMessage message)
        {
            var topic = message.Topic;
            var payloadAsString = Encoding.UTF8.GetString(message.Payload);
            BackdoorEvent?.Invoke(this, new BackdoorEventArgs(topic, payloadAsString));
        }
    }
}
