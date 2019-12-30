using System;
using CommandMessenger;
using System.Threading;

namespace TobiiJoystick
{
    public class SimJoystickController : ArduinoController
    {
        enum Command
        {
            // Commands
            kAcknowledge, // Command to acknowledge that cmd was received
            kError, // Command to report errors
            kVer,
            kVerResult,
            kButton,
            kDone,
            kButtonState,
        };

        protected override void AttachCommandCallBacks()
        {
            base.AttachCommandCallBacks();

        }
        public void DoButton(int buttonIndex, int buttonState)
        {
            var command = new SendCommand((int)Command.kButton, (int)Command.kButtonState, 1000);
            command.AddArgument(buttonIndex);
            command.AddArgument(buttonState);

            // Send command
            var okcmd = _cmdMessenger.SendCommand(command);

            // Check if received a (valid) response
            if (okcmd.Ok)
            {
                Thread.Sleep(100);
                int index = okcmd.ReadInt16Arg();
                int state = okcmd.ReadInt16Arg();
            }
            else
                throw new ApplicationException("No response from controller!");
        }
    }
}