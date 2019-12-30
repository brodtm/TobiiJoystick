// *** ArduinoController ***

// This example expands the SendandReceiveArguments example. The PC will now sends commands to the Arduino when the trackbar 
// is pulled. Every TrackBarChanged events will queue a message to the Arduino to set the blink speed of the 
// internal / pin 13 LED
// 
// This example shows how to :
// - use in combination with WinForms
// - use in combination with ZedGraph
// - send queued commands
// - Use the CollapseCommandStrategy

using System;
using System.Windows.Forms;
using CommandMessenger;
using CommandMessenger.Queue;
using CommandMessenger.Transport.Serial;

namespace TobiiJoystick
{
    enum Command
    {
        Acknowledge,            // Command to acknowledge a received command
        Error,                  // Command to message that an error has occurred

        kVer,
        kVerResult,

        SetLed,                 // Command to turn led ON or OFF
        SetLedFrequency,        // Command to set led blink frequency
    };

    public class ArduinoController //: ISetup
    {

        public class VersionInfo
        {
            public int major=0;
            public int minor=0;
            public string name="";

            public override string ToString()
            {
                return string.Format("{0} {1}.{2}", name, major, minor);
            }

        }
        // This class (kind of) contains presentation logic, and domain model.
        // ChartForm.cs contains the view components 

        protected SerialTransport   _serialTransport;
        protected CmdMessenger      _cmdMessenger;
        protected Form    _controllerForm;

        VersionInfo vi = new VersionInfo();

        public string Name
        {
            get { return this.GetType().Name; }
        }

        // ------------------ MAIN  ----------------------

        // Setup function
        public void Setup(Form controllerForm, string port)
        {
            //var logger = NLog.LogManager.GetCurrentClassLogger();
            //logger.Info("Setup {0} {1}...", this.Name, port);
            // storing the controller form for later reference
            _controllerForm = controllerForm;

            // Create Serial Port object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = port, BaudRate = 115200, DtrEnable = true } // object initializer
            };
            if (_cmdMessenger == null)
            {
                // Initialize the command messenger with the Serial Port transport layer
                // Set if it is communicating with a 16- or 32-bit Arduino board
                _cmdMessenger = new CmdMessenger(_serialTransport, BoardType.Bit16);

                // Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
                _cmdMessenger.ControlToInvokeOn = _controllerForm;

                // Attach the callbacks to the Command Messenger
                AttachCommandCallBacks();

                // Attach to NewLinesReceived for logging purposes
                _cmdMessenger.NewLineReceived += NewLineReceived;

                // Attach to NewLineSent for logging purposes
                _cmdMessenger.NewLineSent += NewLineSent;
            }

            // Start listening
            if (!_cmdMessenger.Connect())
            {
                throw new ApplicationException(string.Format("Unable to connect to {0}", port));
            }

            //_controllerForm.SetLedState(true);
            //_controllerForm.SetFrequency(2);
        }

        // Exit function
        public void Exit()
        {
            // Stop listening
            _cmdMessenger.Disconnect();

            // Dispose Command Messenger
            _cmdMessenger.Dispose();

            // Dispose Serial Port object
            _serialTransport.Dispose();
        }

        /// Attach command call backs. 
        protected virtual void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);
            _cmdMessenger.Attach((int)Command.Acknowledge, OnAcknowledge);
            _cmdMessenger.Attach((int)Command.Error, OnError);
            _cmdMessenger.Attach((int)Command.kVerResult, OnVersion);
        }

        // ------------------  CALLBACKS ---------------------

        // Called when a received command has no attached function.
        // In a WinForm application, console output gets routed to the output panel of your IDE
        protected virtual void OnUnknownCommand(ReceivedCommand arguments)
        {            
            Console.WriteLine(@"Command without attached callback received");
        }

        // Callback function that prints that the Arduino has acknowledged
        protected virtual void OnAcknowledge(ReceivedCommand arguments)
        {
            Console.WriteLine(@" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        protected virtual void OnError(ReceivedCommand arguments)
        {
            Console.WriteLine(@"Arduino has experienced an error");
        }
        // Callback function that prints that the Arduino has acknowledged
        protected virtual void OnVersion(ReceivedCommand arguments)
        {
            vi = new VersionInfo();
            vi.name = arguments.ReadStringArg();
            vi.major = arguments.ReadInt32Arg();
            vi.minor = arguments.ReadInt32Arg();

            //var logger = NLog.LogManager.GetCurrentClassLogger();
            //logger.Info(vi.ToString());
        }
        // Log received line to console
        private void NewLineReceived(object sender, CommandEventArgs e)
        {
            Console.WriteLine(@"Received > " + e.Command.CommandString());
        }

        // Log sent line to console
        private void NewLineSent(object sender, CommandEventArgs e)
        {
            Console.WriteLine(@"Sent > " + e.Command.CommandString());
        }

        // Sent command to change led blinking frequency
        public void SetLedFrequency(double ledFrequency)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.SetLedFrequency,ledFrequency);

            // Put the command on the queue and wrap it in a collapse command strategy
            // This strategy will avoid duplicates of this certain command on the queue: if a SetLedFrequency command is
            // already on the queue when a new one is added, it will be replaced at its current queue-position. 
            // Otherwise the command will be added to the back of the queue. 
            // 
            // This will make sure that when the slider raises a lot of events that each send a new blink frequency, the 
            // embedded controller will not start lagging.
            _cmdMessenger.QueueCommand(new CollapseCommandStrategy(command));
        }


        // Sent command to change led on/of state
        public void SetLedState(bool ledState)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.SetLed, ledState);

            // Send command
            _cmdMessenger.SendCommand(new SendCommand((int)Command.SetLed, ledState));         
        }
    }
}
