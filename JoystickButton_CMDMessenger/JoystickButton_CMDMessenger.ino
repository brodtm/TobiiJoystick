// This code requires the following Arduino libraries:
//requires https://github.com/MHeironimus/ArduinoJoystickLibrary
//requires https://github.com/thijse/Arduino-CmdMessenger

// This application is used as a bridge between the TobiiJoystick C# software, and any assistive device
// application that uses joystick button presses as inputs. The Arduino emulates a USB joystick. When it receives
// commands via USB serial, it sends joystick button press events.
//
// This must be run on the Arduino Leonardo (not an Uno).
//
// by Mordechai Brodt
// 2019-12-30
//--------------------------------------------------------------------

#include <Joystick.h>
#include <CmdMessenger.h>  // CmdMessenger

#define SW_VERSION_MAJOR 1
#define SW_VERSION_MINOR 2

Joystick_ Joystick;
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received.
// In order to receive, attach a callback function to these events
enum
{
  // Commands
  kAcknowledge         , // Command to acknowledge that cmd was received
  kError               , // Command to report errors
  kVer,
  kVerResult,
  kButton,
  kDone,
  kButtonState,
};
// Commands we send from the PC and want to receive on the Arduino.
// We must define a callback function in our Arduino program for each entry in the list below.

void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kVer, KVER);
  cmdMessenger.attach(kButton, KBUTTON);

}
// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError, "Command without attached callback");
}

// Callback function that responds that Arduino is ready (has booted up)
void OnArduinoReady()
{
  cmdMessenger.sendCmd(kAcknowledge, "Arduino ready");
}
void setup() {

  Serial.begin(115200);  // We initialize serial connection

  // Initialize Joystick Library
  Joystick.begin(false);

  // Adds newline to every command
  cmdMessenger.printLfCr();

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Send the status to the PC that says the Arduino has booted
  cmdMessenger.sendCmd(kAcknowledge, "Arduino has started!");
  KVER();

  for (int index = 0; index < 4; index++)
  {
    Joystick.setButton(index, 0);
  }
}
void KVER()
{
  // Send back the result of the addition
  cmdMessenger.sendCmdStart(kVerResult);
  cmdMessenger.sendCmdArg("JOYSTICK_CMD");
  cmdMessenger.sendCmdArg(SW_VERSION_MAJOR);
  cmdMessenger.sendCmdArg(SW_VERSION_MINOR);
  cmdMessenger.sendCmdEnd();
}

void loop() {
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
}

void KBUTTON()
{
  int buttonindex = 0;
  int buttonstate = 0;

  buttonindex = cmdMessenger.readInt16Arg();
  buttonstate = cmdMessenger.readInt16Arg();
  Joystick.setButton((uint8_t)buttonindex, (uint8_t)buttonstate);

  cmdMessenger.sendCmdStart(kButtonState);
  cmdMessenger.sendCmdArg(buttonindex);
  cmdMessenger.sendCmdArg(buttonstate);
  cmdMessenger.sendCmdEnd();
  Joystick.sendState();
}
void OK()
{
  // Send back the result of the addition
  cmdMessenger.sendCmdStart(kDone);
  cmdMessenger.sendCmdEnd();
}
