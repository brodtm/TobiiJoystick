using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tobii.Interaction;

namespace TobiiJoystick
{
    public enum EyeSide
    {
        None,
        Left,
        Right
    }
    public class BlinkInfo
    {
        public EyeSide side;
        public DateTime timestamp;
        public BlinkInfo(EyeSide side, DateTime time)
        {
            this.side = side;
            this.timestamp = time;
        }

    }
    public enum BlinkState
    {
        None,
        Closed,
        Opened,
    }
    class EyeState
    {
        public BlinkState bs;
        public DateTime time = DateTime.MinValue;
    }
    public class BlinkDetector
    {
        public delegate void BlinkDetectHandler(BlinkInfo bi);
        EyePositionData lasteye = null;
        int min_blink_msec = 100;
        int max_blink_msec = 1000;
        int post_blink_msec = 200;
        BlinkInfo pendingBlinkLeft = null;
        BlinkInfo pendingBlinkRight = null;

        Dictionary<EyeSide, EyeState> states = null;
        public BlinkDetector()
        {
            states = new Dictionary<EyeSide, EyeState>();
            states.Add(EyeSide.Left, new EyeState());
            states.Add(EyeSide.Right, new EyeState());
        }


        public void Eval(EyePositionData eyePosition)
        {
            DateTime now = DateTime.Now;
            ///a blink is only valid if the blinked eye is closed for a duration within the min and max values, and the other eye remains open
            ///for the duration of the blink. 
            if (lasteye != null)
            {
                if (EyePositionDataChanged(eyePosition, lasteye))
                {
                    if (eyePosition.HasLeftEyePosition != lasteye.HasLeftEyePosition)
                    {
                        if (eyePosition.HasLeftEyePosition)
                        {
                            if (states[EyeSide.Left].bs == BlinkState.Closed)
                            {
                                TimeSpan elapsed = now - states[EyeSide.Left].time;
                                if ((elapsed.TotalMilliseconds > min_blink_msec) && (elapsed.TotalMilliseconds < max_blink_msec))
                                {
                                    //did a blink!
                                    //Console.WriteLine("Blinked Left");
                                    pendingBlinkLeft = new BlinkInfo(EyeSide.Left, now);
                                }
                                else
                                {
                                    //invalid blink
                                }
                            }
                        }

                        states[EyeSide.Left].time = now;
                        states[EyeSide.Left].bs = eyePosition.HasLeftEyePosition ? BlinkState.Opened : BlinkState.Closed;
                    }
                    if (eyePosition.HasRightEyePosition != lasteye.HasRightEyePosition)
                    {
                        if (eyePosition.HasRightEyePosition)
                        {
                            if (states[EyeSide.Right].bs == BlinkState.Closed)
                            {
                                TimeSpan elapsed = now - states[EyeSide.Right].time;
                                if ((elapsed.TotalMilliseconds > min_blink_msec) && (elapsed.TotalMilliseconds < max_blink_msec))
                                {
                                    //did a blink!
                                    //Console.WriteLine("Blinked Right");
                                    pendingBlinkRight = new BlinkInfo(EyeSide.Right, now);
                                }
                                else
                                {
                                    //invalid blink
                                }
                            }
                        }

                        states[EyeSide.Right].time = now;
                        states[EyeSide.Right].bs = eyePosition.HasRightEyePosition ? BlinkState.Opened : BlinkState.Closed;
                    }
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
                }
            }
            lasteye = eyePosition;

            if ((pendingBlinkLeft != null) && (pendingBlinkRight != null))
            {
                ///pendingBlinkLeft = null;
                //pendingBlinkRight = null;
            }
            if (pendingBlinkLeft != null)
            {
                TimeSpan elapsed = now - pendingBlinkLeft.timestamp;
                if (elapsed.TotalMilliseconds > post_blink_msec)
                {
                    // Console.WriteLine("Blinked Left");
                    pendingBlinkLeft = null;
                }
            }
            if (pendingBlinkRight != null)
            {
                TimeSpan elapsed = now - pendingBlinkRight.timestamp;
                if (elapsed.TotalMilliseconds > post_blink_msec)
                {
                    //Console.WriteLine("Blinked Right");
                    pendingBlinkRight = null;
                }
            }
        }

        static bool EyePositionDataChanged(EyePositionData now, EyePositionData prev)
        {
            if (now.HasLeftEyePosition != prev.HasLeftEyePosition)
                return true;
            if (now.HasRightEyePosition != prev.HasRightEyePosition)
                return true;
            return false;
        }
    }
}