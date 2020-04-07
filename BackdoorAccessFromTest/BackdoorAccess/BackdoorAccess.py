import paho.mqtt.client as mqtt


class BackdoorAccess:
    def __init__(self, broker_host: str = 'localhost', broker_port: int = 1883, broker_source_address: int = 60, base_topic: str = 'Backdoor'):
        self.base_topic = base_topic

        self.client = mqtt.Client()
        self.client.connect(broker_host, broker_port, broker_source_address)
        self.client.loop_start()

    def backdoor(self, sub_topic, payload):
        self.send_message(topic=f'{self.base_topic}/{sub_topic}', payload=payload)

    def send_message(self, topic: str, payload):
        print(f'send_message. {topic} = {payload}')
        message_info: mqtt.MQTTMessageInfo = self.client.publish(topic, payload, qos=0, retain=False)
        message_info.wait_for_publish()

    def __exit__(self, exc_type, exc_value, traceback):
        try:
            self.client.disconnect()
            self.client.loop_stop()
        except:
            pass
