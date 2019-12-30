# TobiiJoystick
Eye gaze to joystick button presses

This software is used to simulate joystick button presses when the user moves their eye gaze from the screen to areas just outside the screen. Configuration settings allow the mapping of these areas to specific button presses. These button presses may be used with software such as [https://thinksmartbox.com/product/grid-3/]

When the software detects that the specific gaze events, it sends messages to an Arduino Leonardo which acts as a serial-joystick bridge device; this device emulates a USB joystick.

These videos show how the software may be used.

[http://www.youtube.com/watch?v=9blltbe84f4]

[https://www.youtube.com/watch?v=A-eIv8kkefo&feature=youtu.be]

    
## Warning:

The software has not been extensively tested. **Use it at your own risk. Rapid eye movements may cause discomfort or other side effects such as vertigo.** Start with short sessions to see how you react.
 

## Prerequisites:

The software uses the Tobii SDK in order to interact with the Tobii eye tracking device.

[https://github.com/Tobii/CoreSDK]

[https://developer.tobii.com/consumer-eye-trackers/core-sdk/getting-started/]

**Tobii Eye Tracking Core Software v2.13.4**

[https://tobiigaming.com/getstarted/?bundle=tobii-core]

**Visual Studio - Community Edition**

[https://visualstudio.microsoft.com/downloads/]

(needed to compile the code)

**CmdMessenger**
The code from 
https://github.com/thijse/Arduino-Code-and-Libraries/tree/master/Libraries/CmdMessenger
has been included for convenience.

**Arduino IDE**
Download the Arduino IDE [https://www.arduino.cc/en/main/software]([https://www.arduino.cc/download_handler.php?f=/arduino-1.8.8-windows.exe]. Install, and allow it to install drivers.

Program the Arduino with tobiijoystick\JoystickButton_CMDMessenger\JoystickButton_CMDMessenger.ino

Note the libraries at the top of this file must be installed as well.

