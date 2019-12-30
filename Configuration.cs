using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TobiiJoystick
{
    public class Configuration
    {
        Dictionary<TargetLocation, int> buttonMap = new Dictionary<TargetLocation, int>();
        string comPort = "COM3";
        [Category("Communication")]
        public string ComPort { get => comPort; set => comPort = value; }
        [JsonIgnore]
        [Browsable(false)]
        public Dictionary<TargetLocation, int> ButtonMap { get => buttonMap; set => buttonMap = value; }
        [Category("Dimensions")]
        public int ScreenWidth { get => screenWidth; set => screenWidth = value; }
        [Category("Dimensions")]
        public int ScreenHeight { get => screenHeight; set => screenHeight = value; }
        [Category("Dimensions")]
        public double BufferPercentX { get => bufferPercentX; set => bufferPercentX = value; }
        [Category("Dimensions")]
        public double BufferPercentY { get => bufferPercentY; set => bufferPercentY = value; }
        [Category("Buttons")] public int Button_TopLeft { get => button_TopLeft; set => button_TopLeft = value; }
        [Category("Buttons")] public int Button_Top { get => button_Top; set => button_Top = value; }
        [Category("Buttons")] public int Button_TopRight { get => button_TopRight; set => button_TopRight = value; }
        [Category("Buttons")] public int Button_RightUpper { get => button_RightUpper; set => button_RightUpper = value; }
        [Category("Buttons")] public int Button_Right { get => button_Right; set => button_Right = value; }
        [Category("Buttons")] public int Button_RightLower { get => button_RightLower; set => button_RightLower = value; }
        [Category("Buttons")] public int Button_BottomRight { get => button_BottomRight; set => button_BottomRight = value; }
        [Category("Buttons")] public int Button_Bottom { get => button_Bottom; set => button_Bottom = value; }
        [Category("Buttons")] public int Button_BottomLeft { get => button_BottomLeft; set => button_BottomLeft = value; }
        [Category("Buttons")] public int Button_LeftLower { get => button_LeftLower; set => button_LeftLower = value; }
        [Category("Buttons")] public int Button_Left { get => button_Left; set => button_Left = value; }
        [Category("Buttons")] public int Button_LeftUpper { get => button_LeftUpper; set => button_LeftUpper = value; }
        [Category("Buttons")] public int Button_Corner_Upper_Left { get => button_Corner_Upper_Left; set => button_Corner_Upper_Left = value; }
        [Category("Buttons")] public int Button_Corner_Upper_Right { get => button_Corner_Upper_Right; set => button_Corner_Upper_Right = value; }
        [Category("Buttons")] public int Button_Corner_Lower_Left { get => button_Corner_Lower_Left; set => button_Corner_Lower_Left = value; }
        [Category("Buttons")] public int Button_Corner_Lower_Right { get => button_Corner_Lower_Right; set => button_Corner_Lower_Right = value; }

        int button_TopLeft = 0;
        int button_Top = 0;
        int button_TopRight = 0;

        int button_RightUpper = 0;
        int button_Right = 0;
        int button_RightLower = 0;

        int button_BottomRight = 0;
        int button_Bottom = 0;
        int button_BottomLeft = 0;

        int button_LeftLower = 0;
        int button_Left = 0;
        int button_LeftUpper = 0;

        int button_Corner_Upper_Left = 0;
        int button_Corner_Upper_Right = 0;
        int button_Corner_Lower_Left = 0;
        int button_Corner_Lower_Right = 0;

        int screenWidth = 2160;
        int screenHeight = 1440;

        double bufferPercentX = 5;//025;
        double bufferPercentY = 5;

        public void StuffMap()
        {
            buttonMap.Clear();

            buttonMap.Add(TargetLocation.Top, button_Top);//click
            buttonMap.Add(TargetLocation.TopLeft, button_TopLeft);//up level
            buttonMap.Add(TargetLocation.TopRight, button_TopRight);//back

            buttonMap.Add(TargetLocation.Right, button_Right);//switchdirection
            buttonMap.Add(TargetLocation.RightLower, button_RightLower);
            buttonMap.Add(TargetLocation.RightUpper, button_RightUpper);

            buttonMap.Add(TargetLocation.Bottom, button_Bottom);//back
            buttonMap.Add(TargetLocation.BottomLeft, Button_BottomLeft);//up level
            buttonMap.Add(TargetLocation.BottomRight, button_BottomRight);//stop

            buttonMap.Add(TargetLocation.Left, button_Left);//up a level
            buttonMap.Add(TargetLocation.LeftUpper, button_LeftUpper);//
            buttonMap.Add(TargetLocation.LeftLower, button_LeftLower);//

            buttonMap.Add(TargetLocation.Corner_Lower_Left, Button_Corner_Lower_Left);//
            buttonMap.Add(TargetLocation.Corner_Upper_Left, Button_Corner_Upper_Left);//
            buttonMap.Add(TargetLocation.Corner_Lower_Right, Button_Corner_Lower_Right);//
            buttonMap.Add(TargetLocation.Corner_Upper_Right, Button_Corner_Upper_Right);//        
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Configuration:");
            sb.AppendLine(string.Format("ScreenSizeX = {0}", screenWidth));
            sb.AppendLine(string.Format("ScreenSizeY = {0}", screenHeight));
            sb.AppendLine("Button Map:");
            foreach (var kvp in buttonMap)
            {
                sb.AppendLine(string.Format("{0} = {1}", kvp.Key, kvp.Value));
            }
            return sb.ToString();
        }
    }
}