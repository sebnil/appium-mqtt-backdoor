using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using MQTTBackdoor;
using Xamarin.Essentials;

namespace BackdoorExampleApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            ButtonLabel.Text = $"Button has not been pressed yet.";

            // Initialize the MQTT backdoor
            Task t = Backdoor.Instance.Initialize(mqttHost: "<mqtt broker address>");

            // Handle backdoor events
            Backdoor.Instance.BackdoorEvent += HandleBackdoorEvent;
        }

        private int numberOfButtonPresses = 0;

        private void HandleBackdoorEvent(object sender, BackdoorEventArgs e)
        {
            // here is where you implement the backdoors
            if (e.Subtopic == "ReceiveNotification")
            {
                DisplayPopupMessage(e.Payload);
            }
        }

        private void DisplayPopupMessage(string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Alert", message, "OK");
            });
        }

        private void HandleButtonClick(object sender, EventArgs e)
        {
            numberOfButtonPresses++;
            if (numberOfButtonPresses == 1)
            {
                ButtonLabel.Text = "Button was pressed once";
            }
            else
            {
                ButtonLabel.Text = $"Button was pressed {numberOfButtonPresses} times";
            }

        }
    }
}
