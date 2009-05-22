using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoblinXNA;
using GoblinXNA.Device.Capture;
using Microsoft.Xna.Framework;

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
        public static bool          BigOneOptimize      { get; set; }
        public static int           BigOneMaxDropouts   { get; set; }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Building Scales
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static float EditableScale { get; set; }
        public static float RealScale { get; set; }
        public static float AirAdjustment { get; set; }
        public static float GroundToFootRatio { get; set; }
        public static float GraphX { get; set; }
        public static float GraphY { get; set; }
        public static float GraphZ { get; set; }
        public static float AdditionalAirRights { get; set; }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Interaction Params
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static float WandMovementThreshold            { get; set; }
        public static float MinDistanceForGrab               { get; set; }
        public static float VotesForChangingSelectedBuilding { get; set; }
        public static float MinGestureDistance               { get; set; }
        public static double MinTimeBetweenMenuSelections    { get; set; }
        public static int    UpdateFreq                      { get; set; }
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Miscellanous
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static bool          BuildingsDetailed   { get; set; }
        public static float         ModelScale          { get; set; }
        public static String        ModelDir            { get; set; }
        public static Vector3       ModelBaseTrans      { get; set; }
        public static Quaternion    ModelBaseRot        { get; set; }


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
            BigOneOptimize = bool.Parse(State.GetSettingVariable("BigOneOptimize"));
            BigOneMaxDropouts = int.Parse(State.GetSettingVariable("BigOneMaxDropouts"));

            EditableScale = float.Parse(State.GetSettingVariable("EditableScale"));
            RealScale = float.Parse(State.GetSettingVariable("RealScale"));
            AirAdjustment = float.Parse(State.GetSettingVariable("AirAdjustment"));
            GroundToFootRatio = float.Parse(State.GetSettingVariable("GroundToFootRatio"));
            GraphX = float.Parse(State.GetSettingVariable("GraphX"));
            GraphY = float.Parse(State.GetSettingVariable("GraphY"));
            GraphZ = float.Parse(State.GetSettingVariable("GraphZ"));
            AdditionalAirRights = float.Parse(State.GetSettingVariable("AdditionalAirRights"));

            WandMovementThreshold = float.Parse(State.GetSettingVariable("WandMovementThreshold"));
            MinDistanceForGrab = float.Parse(State.GetSettingVariable("MinDistanceForGrab"));
            VotesForChangingSelectedBuilding = float.Parse(State.GetSettingVariable("VotesForChangingSelectedBuilding"));
            MinGestureDistance = float.Parse(State.GetSettingVariable("MinGestureDistance"));
            MinTimeBetweenMenuSelections = double.Parse(State.GetSettingVariable("MinTimeBetweenMenuSelections"));
            UpdateFreq = int.Parse(State.GetSettingVariable("UpdateFreq"));

            BuildingsDetailed = bool.Parse(State.GetSettingVariable("BuildingsDetailed"));

            if (BuildingsDetailed)
            {
                ModelScale = 0.0073f;
                ModelDir = "Detailed/";
                ModelBaseTrans = new Vector3(-33.5f, -54.25f, 0);
                ModelBaseRot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
            }
            else
            {
                ModelScale = 0.00728f;
                ModelDir = "Plain/";
                ModelBaseTrans = new Vector3(-12.5f, -15.69f, 0);
                ModelBaseRot = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180);
            }
        }

    }

}
