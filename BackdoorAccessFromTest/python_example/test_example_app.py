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
