using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GoblinXNA.Helpers;
using Microsoft.Xna.Framework.Graphics;

namespace Manhattanville
{
    class ModificationManager
    {
        private static Manhattanville app;
        private static GraphicsDeviceManager graphics;
        private static Vector3 startingLocation = Vector3.Zero;
        private static Vector3 currentLocation = Vector3.Zero;
        private static Vector3 previousLocation = Vector3.Zero;
        private static double distanceCovered = 0.0;
        private static Vector3 initialScale;
        private static float airRights;
        private static float oldAirRights;
        private static int stories;
        private static Vector3 centerOfCeilWithOffset;
        private static bool lastUp = true;
        private static int directionChangeCounter = 0;

        public static void initialize(Manhattanville m, GraphicsDeviceManager g)
        {
            app = m;
            graphics = g;
        }

        public static bool grabHandle()
        {
            if (app.selectedHandle == null) return false;
 
            initialScale = app.selectedBuilding.EditBuildingTransform.Scale;
            airRights = app.selectedBuilding.EditBuildingTransform.ModelBuilding.Lot.airRights;
            oldAirRights = app.selectedBuilding.EditBuildingTransform.ModelBuilding.Lot.previousAirRights;
            stories = app.selectedBuilding.EditBuildingTransform.Stories;
            centerOfCeilWithOffset = app.selectedBuilding.CenterOfCeilWithOffset;
            GoblinXNA.UI.Notifier.AddMessage("Grabbing " + app.selectedHandle.Name);
            startingLocation = currentLocation = previousLocation = app.getWandLocation();

            return true;
        }

        public static void accept()
        {
            releaseHandle();
        }

        public static void reject()
        {
            releaseHandle();
            app.selectedBuilding.EditBuildingTransform.ModelBuilding.Lot.airRights = airRights;
            app.selectedBuilding.EditBuildingTransform.ModelBuilding.Lot.previousAirRights = oldAirRights;
            app.selectedBuilding.EditBuildingTransform.Stories = stories;
            app.selectedBuilding.CenterOfCeilWithOffset = centerOfCeilWithOffset;
            app.selectedBuilding.EditBuildingTransform.Scale = initialScale;
            app.selectedBuilding.EditBuildingTransform.broadcast();
        }

        public static bool releaseHandle()
        {
            if (app.selectedHandle == null)
                return false;
            else
                GoblinXNA.UI.Notifier.AddMessage("Releasing " + app.selectedHandle.Name);

            return true;
        }

        public static Vector3 calcDelta()
        {
            return (app.getWandLocation() - startingLocation);
        }

        public static void processWandMovement()
        {
            previousLocation = currentLocation;
            currentLocation = app.getWandLocation();

            switch (app.selectedHandle.Loc)
            {
                case Handle.Location.Top:
                    
                    bool up = (currentLocation.Z - app.selectedHandle.getLocation().Z) > 3;

                    if (up != lastUp)
                    {
                        directionChangeCounter++;

                        if (directionChangeCounter < 5) return;

                        directionChangeCounter = 0;
                        distanceCovered = 0;
                        lastUp = up;

                        if (up)
                        {
                            GoblinXNA.UI.Notifier.AddMessage("Keep waving wand UPWARD ABOVE the handle to ADD a floor");
                        }
                        else
                        {
                            GoblinXNA.UI.Notifier.AddMessage("Keep waving wand DOWNWARD BELOW the handle to REMOVE a floor");
                        }
                    }

                    double d = currentLocation.Z - previousLocation.Z;

                    if (up)
                    {
                        // Up
                        if (d > 0)
                        {
                            distanceCovered += d;
                            if (distanceCovered > Settings.WandMovementThreshold)
                            {
                                addFloor(1);
                                distanceCovered = 0;
                            }
                            //GoblinXNA.UI.Notifier.AddMessage("UP, distanceCovered=" + distanceCovered);
                        }
                    }
                    else
                    {
                        // Down
                        if (d < 0)
                        {
                            distanceCovered += (-d);
                            if (distanceCovered > Settings.WandMovementThreshold)
                            {
                                addFloor(-1);
                                distanceCovered = 0;
                            }
                            //GoblinXNA.UI.Notifier.AddMessage("DOWN, distanceCovered=" + distanceCovered);
                        }
                    }
                    
                    /*
                    if (calcDelta().Z > Settings.WandMovementThreshold)
                    {
                        addFloor(1);
                        startingLocation = app.getWandLocation();
                    }
                    else if (calcDelta().Z < -Settings.WandMovementThreshold)
                    {
                        addFloor(-1);
                        startingLocation = app.getWandLocation();
                    }
                    */
                    break;
                case Handle.Location.BottomNE:
                    break;
                case Handle.Location.BottomNW:
                    break;
                case Handle.Location.BottomSW:
                    break;
                case Handle.Location.BottomSE:
                    break;
            }
        }

        internal static void addFloor(int floors)
        {
            // TODO: We should probably convert numeric data to numeric variables
            // at load time
            app.pop.Play();

            int currStories = app.selectedBuilding.Stories;
            int newStories = currStories + floors;
            float heightRatio = 1f;

            if (newStories < 0)
            {
                // We probably shouldn't allow negative floors
                return;
            }

            Vector3 scaleVector = app.selectedBuilding.EditBuildingTransform.Scale;
            Vector3 ceilVector = app.selectedBuilding.CenterOfCeilWithOffset;

            if (currStories != 0)
            {
                heightRatio = (float)newStories / (float)currStories;

                scaleVector.Z = scaleVector.Z * heightRatio;
                ceilVector.Z = ceilVector.Z * heightRatio;
                Log.Write("ceilVector.Z=" + ceilVector.Z + " heightRatio=" + heightRatio);
            }
            else
            {
                heightRatio = (float)newStories / (float)app.selectedBuilding.Lot.stories;
                scaleVector.Z = heightRatio;// *app.scale;
                ceilVector = app.selectedBuilding.CenterOfCeilWithOffsetOrig * heightRatio;
            }

            app.selectedBuilding.Lot.airRights -= (floors * app.selectedBuilding.Lot.footprint);

            app.selectedBuilding.EditBuildingTransform.Scale = scaleVector;

            //Log.Write("CenterOfCeilWithOffset="+Vector3.Transform(app.selectedBuilding.CenterOfCeilWithOffset, app.selectedBuilding.EditBuildingTransform.WorldTransformation).ToString());
            
            //app.selectedBuilding.EditBuildingTransform.Translation = new Vector3(0f, 40f, 5f) + 
            
            app.selectedBuilding.CenterOfCeilWithOffset = ceilVector;
            app.selectedBuilding.EditBuildingTransform.broadcast();
            app.selectedBuilding.Stories = newStories;

            app.selectedBuilding.DataChanged = true;
            if (AppStateMgr.inState(AppState.Info)) app.dataRepresentation.renderData(app.selectedBuilding);

            Log.Write("editableBuildingTransformNode.Scale="
                + app.selectedBuilding.EditBuildingTransform.Scale.ToString() + "\n");

            GoblinXNA.UI.Notifier.AddMessage(
                app.selectedBuilding.Name + " now has "
                + app.selectedBuilding.Stories + " stories.");

            Texture2D old = app.selectedBuilding.RenderedAirRights;

            app.selectedBuilding.RenderedAirRights = Data.txt2Txt(
                           app.GraphicsDevice,
                           "   Available Air Rights: " + app.selectedBuilding.Lot.airRights.ToString("#,###"),
                           240, 26, Manhattanville.infoFont, Manhattanville.infoColor);

            app.airRightLabelGeo.Material.Texture = app.selectedBuilding.RenderedAirRights;

            old.Dispose();
        
        }
    }
}
