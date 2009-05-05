﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA;
using GoblinXNA.Device.Capture;

namespace Manhattanville
{
    class Settings
    {
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Camera
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static System.Int32  CameraID            { get; set; }
        public static float         CameraFx            { get; set; }
        public static float         CameraFy            { get; set; }
        public static Resolution    Resolution          { get; set; }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Debug
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static bool          ShowNotifications   { get; set; }
        public static int           FadeOutTime         { get; set; }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Markers
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static bool          GroundOptimize      { get; set; }
        public static int           GroundMaxDropouts   { get; set; }
        public static bool          ToolOptimize        { get; set; }
        public static int           ToolMaxDropouts     { get; set; }
        public static float         ToolSmoother        { get; set; }
        public static String        ToolTagName         { get; set; }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Building Scales
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static float EditableScale { get; set; }
        public static float RealScale { get; set; }        
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Miscellanous
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static bool          BuildingsDetailed   { get; set; }
        public static bool          BuildingsSubset     { get; set; }


        public static void loadFromSettingsFile()
        {

            CameraID = Int32.Parse(State.GetSettingVariable("CameraID"));
            CameraFx = float.Parse(State.GetSettingVariable("CameraFx"));
            CameraFy = float.Parse(State.GetSettingVariable("CameraFy"));
            Resolution = (Resolution)Enum.Parse(typeof(Resolution), State.GetSettingVariable("Resolution"));
            
            ShowNotifications = bool.Parse(State.GetSettingVariable("ShowNotifications"));
            FadeOutTime = int.Parse(State.GetSettingVariable("FadeOutTime"));

            GroundMaxDropouts = int.Parse(State.GetSettingVariable("GroundMaxDropouts"));
            ToolMaxDropouts = int.Parse(State.GetSettingVariable("ToolMaxDropouts"));
            GroundOptimize = bool.Parse(State.GetSettingVariable("GroundOptimize"));
            ToolOptimize = bool.Parse(State.GetSettingVariable("ToolOptimize"));
            ToolSmoother = float.Parse(State.GetSettingVariable("ToolSmoother"));
            ToolTagName = State.GetSettingVariable("ToolTagName");

            EditableScale = int.Parse(State.GetSettingVariable("EditableScale"));
            RealScale = int.Parse(State.GetSettingVariable("RealScale"));

            BuildingsDetailed = bool.Parse(State.GetSettingVariable("BuildingsDetailed"));
            BuildingsSubset = bool.Parse(State.GetSettingVariable("BuildingsSubset"));
        }

    }

}
