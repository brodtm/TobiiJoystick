using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Tobii.Interaction;
using Tobii.Interaction.Framework;
//using System.Drawing;

namespace TobiiJoystick
{

    
    
    //enum GazeAt
    //{
    //    None,
    //    In,
    //    Left,
    //    Right,
    //    Up,
    //    Down
    //}
    /// <summary>
    /// The data streams provide nicely filtered eye-gaze data from the eye tracker 
    /// transformed to a convenient coordinate system. The point on the screen where 
    /// your eyes are looking (gaze point), and the points on the screen where your 
    /// eyes linger to focus on something (fixations) are given as pixel coordinates 
    /// on the screen. The positions of your eyeballs (eye positions) are given in 
    /// space coordinates in millimeters relative to the center of the screen.
    /// 
    /// The Fixation data stream provides information about when the user is fixating
    /// his/her eyes at a single location. This data stream can be used to get an 
    /// understanding of where the user’s attention is. In most cases, when a person
    /// is fixating at something for a long time, this means that the person’s brain 
    /// is processing the information at the fixation point.
    /// </summary>
    public class Program_CMD
    {
        public static void Main(string[] args)
        {
            try
            {
                Console.WriteLine(string.Format("{0} {1} v{2}", Application.CompanyName, Application.ProductName, Application.ProductVersion));
                Console.WriteLine();                             

                string cfgfile = Path.Combine(Path.GetDirectoryName(Application.CommonAppDataPath), "config.json");
                Console.WriteLine(string.Format("Config File: {0}", cfgfile));
                Configuration cfg = new Configuration();

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
                    File.WriteAllText(cfgfile, JsonConvert.SerializeObject(cfg));
                }

                if (File.Exists(cfgfile))
                {
                    // read file into a string and deserialize JSON to a type
                    cfg = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(cfgfile));
                }
                Console.WriteLine(cfg.ToString());
                //GazeAt gazeAtLast = GazeAt.None;
                SimJoystickController sjc = new SimJoystickController();
                sjc.Setup(null, cfg.ComPort);
                // Everything starts with initializing Host, which manages the connection to the 
                // Tobii Engine and provides all the Tobii Core SDK functionality.
                // NOTE: Make sure that Tobii.EyeX.exe is running
                var host = new Host();

                // we will create virtual window covering whole screen
                // so we will get screenbounds using States.
                var screenBoundsState = host.States.GetScreenBoundsAsync().Result;
                var screenBounds = screenBoundsState.IsValid
                    ? screenBoundsState.Value
                    : new Rectangle(0d, 0d, 1000d, 1000d);

                // Initialize Fixation data stream.
                var fixationDataStream = host.Streams.CreateFixationDataStream();

                // Because timestamp of fixation events is relative to the previous ones
                // only, we will store them in this variable.
                var fixationBeginTime = 0d;
                System.Drawing.Rectangle screenrect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                screenrect = new System.Drawing.Rectangle(0, 0, cfg.ScreenWidth, cfg.ScreenHeight);
                Targets targets = new Targets(screenrect, cfg.BufferPercentX, cfg.BufferPercentY);// new System.Drawing.Rectangle((int)screenBoundsState.Value.X, (int)screenBoundsState.Value.Y, (int)screenBoundsState.Value.Width, (int)screenBoundsState.Value.Height));
                TargetLocation tl_last = TargetLocation.None;
                TargetLocation tl_last_valid = TargetLocation.None;
                List<int> pressedButtons = new List<int>();
                Console.WriteLine("Running. Press a key to exit application.");

                fixationDataStream.Next += (o, fixation) =>
                {
                    // On the Next event, data comes as FixationData objects, wrapped in a StreamData<T> object.
                    var fixationPointX = fixation.Data.X;
                    var fixationPointY = fixation.Data.Y;
                    //GazeAt gazeAt = GazeAt.In;

                    switch (fixation.Data.EventType)
                    {
                        case FixationDataEventType.Begin:
                            fixationBeginTime = fixation.Data.Timestamp;
                            //Console.WriteLine("Begin fixation at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                            break;

                        case FixationDataEventType.Data:
                            //Console.WriteLine("During fixation, currently at X: {0}, Y: {1}", fixationPointX, fixationPointY);
                            System.Drawing.Point p = new System.Drawing.Point((int)fixationPointX, (int)fixationPointY);
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

                Console.ReadKey();

                // we will close the coonection to the Tobii Engine before exit.
                host.DisableConnection();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
            }
        }
    }
}