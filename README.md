# Appium MQTT Backdoor
![Example use of backdoor in Robot framework](ReadmeAssets/ios.gif)

## Background
Appium is generally considered a blackbox testing tool. It has no access to the app's methods. Appium acts on the elements accessible by an user.

But what if that is not enough? For example if you want to simulate an incoming notifiction, a bluetooth device connection, or a crash? For that you need whitebox testing with access to the app's internal methods.

There are ways to do whitebox testing in Android using the espresso driver:
- https://appiumpro.com/editions/51

There is also a library to add backdoor testing to iOS+Appium:
- https://github.com/alexmx/Insider

But neither of these solutions as a backdoor in C# apps, mainly Android and iOS writting using the Xamarin framework. That is the problem this repo solves.

## Prerequisites
### MQTT broker
You will need access to a MQTT broker (server). The Eclipse Mosquitto broker is good:
- https://mosquitto.org/download/

Install it and open port 1887
- http://www.abrandao.com/2018/03/running-mosquitto-mqtt-on-windows-10-super-easy/

### Appium
Not really necessary for this backdoor to work, but it is a really good tool for testing mobile apps.

### Python
Necessary to call the backdoor from if you want to use the code I wrote, but you could implement the backdoor access in any language. You just need to be able to call a MQTT broker.

### Robot framework
Only necessary if you want to use it. You could run the tests in python or any other language.
- https://robotframework.org/

## Add testing backdoors to your app
### Implement the backdoor in your C# app
Add a handler to the backdoor events
```C#
// Initialize the MQTT backdoor
Task t = Backdoor.Instance.Initialize(mqttHost: "<mqtt server address>");

// Handle backdoor events
Backdoor.Instance.BackdoorEvent += HandleBackdoorEvent;
```
```C#
private void HandleBackdoorEvent(object sender, BackdoorEventArgs e)
{
    // here is where you implement the backdoors
    if (e.Subtopic == "ReceiveNotification")
    {
        DisplayPopupMessage(e.Payload);
    }
}
```

There is an example app implemented in Xamarin forms in /AppBackdoor/BackdoorExampleApp

### Use the backdoor from your tests
There is a python module in appium-mqtt-backdoor\BackdoorAccess\BackdoorAccess

Call it from python like so:
```python
from BackdoorAccess.BackdoorAccess import BackdoorAccess

backdoorAccess = BackdoorAccess(broker_host=MQTT_BROKER_HOST)
backdoorAccess.backdoor('ReceiveNotification', 'Showing popup via backdoor')
```

I made two examples on how to use the backdoor. One in Robot framwork and one in pure python. Both uses the same python module implementation.

#### Robot framwork example:
```robot
*** Settings ***
Library            AppiumLibrary
Library            ../BackdoorAccess/BackdoorAccess.py    <mqtt broker addres>

*** Variables ***
${APPIUM_SERVER}            http://127.0.0.1:4723/wd/hub
${Android apk path}         ${CURDIR}/../../AppBackdoor/BackdoorExampleApp/BackdoorExampleApp.Android/bin/Debug/com.companyname.backdoorexampleapp-Signed.apk
${Android package name}     com.companyname.backdoorexampleapp
${iOS app path}             ${CURDIR}/../../AppBackdoor/BackdoorExampleApp/BackdoorExampleApp.iOS/bin/iPhoneSimulator/Release/BackdoorExampleApp.iOS.app
${Apple team id}            <your team id>

*** Test Cases ***
Test button press and notifications on Android app
    [Tags]  Android
    Open Android example app
    Click button and check that label was updated
    Simulate a notification
	Sleep  2
	Close application

Test button press and notifications on iOS app
    [Tags]  iOS
    Open iOS example app
    Click button and check that label was updated
    Simulate a notification
    Sleep  2
	Close application


*** Keywords ***
Open Android example app
    Open Application    ${APPIUM_SERVER}    platformName=Android    deviceName=Android Emulator    noReset=true    autoGrantPermissions=true  app=${Android apk path}  appPackage=${Android package name}

Open iOS example app
    Open Application    ${APPIUM_SERVER}    platformName=iOS    deviceName=iPhone 8    automationName=XCUITest    noReset=true    platformVersion=13.4    xcodeOrgId=${Apple team id}    xcodeSigningId=iPhone Developer    app=${iOS app path}

Click button and check that label was updated
	Click element  accessibility_id=A button
	Element Should Contain Text  accessibility_id=Number of button clicks  Button was pressed once

Simulate a notification
    Backdoor  ReceiveNotification  Showing popup via backdoor

```

#### Python unittest example
```python
import unittest
import os
from appium import webdriver
from BackdoorAccess.BackdoorAccess import BackdoorAccess
import time

script_folder = os.path.dirname(os.path.realpath(__file__))

MQTT_BROKER_HOST = '<mqtt broker address>'
APPLE_TEAM_ID = 'your team id'

class MyTestCase(unittest.TestCase):
    def test_button_press_and_notifications_on_android(self):
        self.open_android_example_app()
        self.click_button_and_check_that_label_was_updated()
        self.simulate_a_notification()
        time.sleep(2)
        self.driver.close_app()

    def test_button_press_and_notifications_on_ios(self):
        self.open_ios_example_app()
        self.click_button_and_check_that_label_was_updated()
        self.simulate_a_notification()
        time.sleep(2)
        self.driver.close_app()

    def open_android_example_app(self):
        desired_caps = {}
        desired_caps['platformName'] = 'Android'
        desired_caps['deviceName'] = 'Android Emulator'
        desired_caps['noReset'] = True
        desired_caps[
            'app'] = f'{script_folder}/../../AppBackdoor/BackdoorExampleApp/BackdoorExampleApp.Android/bin/Release/com.companyname.backdoorexampleapp-Signed.apk'
        desired_caps['appPackage'] = 'com.companyname.backdoorexampleapp'

        self.driver = webdriver.Remote('http://localhost:4723/wd/hub', desired_caps)

    def open_ios_example_app(self):
        # modify these capabilities to work on your machine
        desired_caps = {}
        desired_caps['platformName'] = 'iOS'
        desired_caps['deviceName'] = 'iPhone 8'
        desired_caps['automationName'] = 'XCUITest'
        desired_caps['noReset'] = True
        desired_caps['platformVersion'] = '13.4'
        desired_caps['xcodeOrgId'] = APPLE_TEAM_ID
        desired_caps['xcodeSigningId'] = 'iPhone Developer'
        desired_caps['app'] = os.path.abspath(f'{script_folder}/../../AppBackdoor/BackdoorExampleApp/BackdoorExampleApp.iOS/bin/iPhoneSimulator/Release/BackdoorExampleApp.iOS.app')

        self.driver = webdriver.Remote('http://localhost:4723/wd/hub', desired_caps)

    def click_button_and_check_that_label_was_updated(self):
        # click button
        button = self.driver.find_element_by_accessibility_id('A button')
        button.click()

        # check that label text was updated
        label = self.driver.find_element_by_accessibility_id('Number of button clicks')
        self.assertEqual('Button was pressed once', label.text)

    def simulate_a_notification(self):
        backdoorAccess = BackdoorAccess(broker_host=MQTT_BROKER_HOST)
        backdoorAccess.backdoor('ReceiveNotification', 'Showing popup via backdoor')


if __name__ == '__main__':
    unittest.main()

```

## Security
Obviously having a backdoor in your application is a data security issue. Disable all backdoors before shipping the application:
```C#
#if BACKDOOR_ENABLED
// Initialize the MQTT backdoor
Task t = Backdoor.Instance.Initialize(mqttHost: "<mqtt server address>");
...
#endif
```

## Support my creation of open source software:
[![Flattr this git repo](http://api.flattr.com/button/flattr-badge-large.png)](https://flattr.com/submit/auto?user_id=sebnil&url=https://github.com/sebnil/appium-mqtt-backdoor)

<a href='https://ko-fi.com/A0A2HYRH' target='_blank'><img height='36' style='border:0px;height:36px;' src='https://az743702.vo.msecnd.net/cdn/kofi2.png?v=0' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a>
