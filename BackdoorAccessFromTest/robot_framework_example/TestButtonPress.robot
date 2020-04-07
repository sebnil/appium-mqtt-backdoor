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
