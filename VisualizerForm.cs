using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
using Wexman.Design;

namespace TobiiJoystick
{
    public partial class VisualizerForm : Form
    {
        Host host = null;
        Configuration cfg = null;
        string cfgfile = "";
        public VisualizerForm()
        {
            InitializeComponent();
        }

        private void VisualizerForm_Load(object sender, EventArgs e)
        {
            //try
            //{
            Console.WriteLine(string.Format("{0} {1} v{2}", Application.CompanyName, Application.ProductName, Application.ProductVersion));
            Console.WriteLine();

            cfgfile = Path.Combine(Path.GetDirectoryName(Application.CommonAppDataPath), "config.json");
            Console.WriteLine(string.Format("Config File: {0}", cfgfile));
            cfg = new Configuration();

            if (File.Exists(cfgfile) == false)//if no config file exists, then load a default config file
            {
                System.Drawing.Rectangle screenrecttmp = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                cfg.ScreenWidth = screenrecttmp.Width;
                cfg.ScreenHeight = screenrecttmp.Height;

                Dictionary<TargetLocation, int> buttonMap = new Dictionary<TargetLocation, int>();
                buttonMap.Add(TargetLocation.Top, 1);//click
                buttonMap.Add(TargetLocation.TopLeft, 0);//up level
                buttonMap.Add(TargetLocation.TopRight, 3);//back

                buttonMap.Add(TargetLocation.Right, 3);//switchdirection
                buttonMap.Add(TargetLocation.RightLower, 0);
                buttonMap.Add(TargetLocation.RightUpper, 0);

                buttonMap.Add(TargetLocation.Bottom, 3);//back
                buttonMap.Add(TargetLocation.BottomLeft, 2);//up level
                buttonMap.Add(TargetLocation.BottomRight, 4);//stop

                buttonMap.Add(TargetLocation.Left, 2);//up a level
                buttonMap.Add(TargetLocation.LeftUpper, 0);//
                buttonMap.Add(TargetLocation.LeftLower, 0);//
                cfg.ButtonMap = buttonMap;
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(cfgfile, JsonConvert.SerializeObject(cfg, Formatting.Indented));
            }

            if (File.Exists(cfgfile))
            {
                // read file into a string and deserialize JSON to a type
                cfg = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(cfgfile));
            }
            cfg.StuffMap();
            Console.WriteLine(cfg.ToString());
            //GazeAt gazeAtLast = GazeAt.None;
            SimJoystickController sjc = new SimJoystickController();
            try
            {
                sjc.Setup(null, cfg.ComPort);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            // Everything starts with initializing Host, which manages the connection to the 
            // Tobii Engine and provides all the Tobii Core SDK functionality.
            // NOTE: Make sure that Tobii.EyeX.exe is running
            host = new Host();

            // we will create virtual window covering whole screen
            // so we will get screenbounds using States.
            //var screenBoundsState = host.States.GetScreenBoundsAsync().Result;
            //var screenBounds = screenBoundsState.IsValid
            //    ? screenBoundsState.Value
            //    : new Rectangle(0d, 0d, 1000d, 1000d);

            // Initialize Fixation data stream.
            var fixationDataStream = host.Streams.CreateFixationDataStream();


            var _eyePositionDataStream  = host.Streams.CreateEyePositionStream();

            // Because timestamp of fixation events is relative to the previous ones
            // only, we will store them in this variable.
            var fixationBeginTime = 0d;
            System.Drawing.Rectangle screenrect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            screenrect = new System.Drawing.Rectangle(0, 0, cfg.ScreenWidth, cfg.ScreenHeight);
            Targets targets = new Targets(screenrect, cfg.BufferPercentX / 100.0, cfg.BufferPercentY / 100.0);// new System.Drawing.Rectangle((int)screenBoundsState.Value.X, (int)screenBoundsState.Value.Y, (int)screenBoundsState.Value.Width, (int)screenBoundsState.Value.Height));
            TargetLocation tl_last = TargetLocation.None;
            TargetLocation tl_last_valid = TargetLocation.None;
            List<int> pressedButtons = new List<int>();
            //Console.WriteLine("Running. Press a key to exit application.");
            this.visualizer1.SetTargets(targets, cfg);

            fixationDataStream.Next += (o, fixation) =>
            {
                // On the Next event, data comes as FixationData objects, wrapped in a StreamData<T> object.
                var fixationPointX = fixation.Data.X;
                var fixationPointY = fixation.Data.Y;
                //GazeAt gazeAt = GazeAt.In;
                visualizer1.SetFixationData(fixation.Data);
                switch (fixation.Data.EventType)
                {
                    case FixationDataEventType.Begin:
                        fixationBeginTime = fixation.Data.Timestamp;
                        //Console.WriteLine("Begin fixation at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                        break;

                    case FixationDataEventType.Data:
                        //Console.WriteLine("During fixation, currently at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                        System.Drawing.Point p = new System.Drawing.Point((int)fixationPointX, (int)fixationPointY);
                        //visualizer1.SetFixationPoint(p);
                        TargetLocation tl = targets.CheckHit(p);
                        if (tl_last != tl)
                        {
                            Console.WriteLine("TargetLocation {0} X: {1}, Y: {2}", tl, fixationPointX, fixationPointY);
                            if ((int)tl > (int)TargetLocation.Screen)
                            {
                                //only if we were at the screen previously
                                if (tl_last_valid == TargetLocation.Screen)
                                {
                                    //do the button
                                    if (cfg.ButtonMap.ContainsKey(tl))
                                    {
                                        int buttonindex = cfg.ButtonMap[tl];
                                        if (buttonindex > 0)
                                        {
                                            pressedButtons.Add(buttonindex - 1);
                                            sjc.DoButton(buttonindex - 1, 1);
                                        }
                                    }
                                }
                            }
                            else if (tl == TargetLocation.Screen)
                            {
                                foreach (int buttonindex in pressedButtons)
                                {
                                    sjc.DoButton(buttonindex, 0);
                                }
                                pressedButtons.Clear();
                            }
                            if (tl != TargetLocation.None)
                            {
                                tl_last_valid = tl;
                            }
                        }
                        tl_last = tl;
                        break;

                    case FixationDataEventType.End:
                        //Console.WriteLine("End fixation at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                        //Console.WriteLine("Fixation duration: {0}",
                        //    fixationBeginTime > 0
                        //        ? TimeSpan.FromMilliseconds(fixation.Data.Timestamp - fixationBeginTime)
                        //        : TimeSpan.Zero);
                        //Console.WriteLine();
                        break;

                    default:
                        throw new InvalidOperationException("Unknown fixation event type, which doesn't have explicit handling.");
                }
            };

            //EyePositionData lasteye = null;
            BlinkDetector blinkDetector = new BlinkDetector();

            _eyePositionDataStream.EyePosition(eyePosition =>
            {
                //if (lasteye != null)
                //{
                    //if (EyePositionDataChanged(eyePosition, lasteye))
                    //{
                        //Console.WriteLine("Has Left eye position: {0}", eyePosition.HasLeftEyePosition);
                        //Console.WriteLine("Left eye position: X:{0} Y:{1} Z:{2}",
                        //    eyePosition.LeftEye.X, eyePosition.LeftEye.Y, eyePosition.LeftEye.Z);
                        //Console.WriteLine("Left eye position (normalized): X:{0} Y:{1} Z:{2}",
                        //    eyePosition.LeftEyeNormalized.X, eyePosition.LeftEyeNormalized.Y, eyePosition.LeftEyeNormalized.Z);

                        //Console.WriteLine("Has Right eye position: {0}", eyePosition.HasRightEyePosition);
                        //Console.WriteLine("Right eye position: X:{0} Y:{1} Z:{2}",
                        //    eyePosition.RightEye.X, eyePosition.RightEye.Y, eyePosition.RightEye.Z);
                        //Console.WriteLine("Right eye position (normalized): X:{0} Y:{1} Z:{2}",
                        //    eyePosition.RightEyeNormalized.X, eyePosition.RightEyeNormalized.Y, eyePosition.RightEyeNormalized.Z);
                        //Console.WriteLine();

                        blinkDetector.Eval(eyePosition);
                    //}
                //}
                //lasteye = eyePosition;
                
               
            });

            //Console.ReadKey();

            // we will close the coonection to the Tobii Engine before exit.
            //host.DisableConnection();
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    Console.WriteLine(ex.StackTrace);
            //    //Console.ReadKey();
            //}
        }
        static bool EyePositionDataChanged(EyePositionData now, EyePositionData prev)
        {
            if (now.HasLeftEyePosition != prev.HasLeftEyePosition)
                return true;
            if (now.HasRightEyePosition != prev.HasRightEyePosition)
                return true;
            return false;
        }
        private void VisualizerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (host != null)
            {
                // we will close the coonection to the Tobii Engine before exit.
                host.DisableConnection();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PropEditor pe = new PropEditor();
            pe.SetProps(cfg);
            DialogResult result = pe.ShowDialog();
            if (result == DialogResult.OK)
            {
                // serialize JSON to a string and then write string to a file
                File.WriteAllText(cfgfile, JsonConvert.SerializeObject(cfg, Formatting.Indented));
            }

            //GenericDictionaryEditor<TargetLocation, System.Drawing.Rectangle> gef = new GenericDictionaryEditor<TargetLocation, System.Drawing.Rectangle>(typeof(TargetLocation));
            //gef.EditValue()
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}