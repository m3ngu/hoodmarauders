using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GoblinXNA.Helpers;

namespace Manhattanville
{
    class ModificationManager
    {

        private static Manhattanville app;
        private static GraphicsDeviceManager graphics;
        private static Vector3 startingLocation = Vector3.Zero;

        public static void initialize(Manhattanville m, GraphicsDeviceManager g)
        {
            app = m;
            graphics = g;
        }

        public static void grabHandle()
        {
            GoblinXNA.UI.Notifier.AddMessage("Grabbing " + app.selectedHandle.Name);
            startingLocation = app.getWandLocation();
        }

        public static void releaseHandle()
        {
            GoblinXNA.UI.Notifier.AddMessage("Releasing " + app.selectedHandle.Name);
        }

        public static Vector3 calcDelta()
        {
            return (app.getWandLocation() - startingLocation);
        }

        public static void processWandMovement()
        {
            switch (app.selectedHandle.Loc)
            {
                case Handle.Location.Top:
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
            app.selectedBuilding.Lot.airRights -= (floors * app.selectedBuilding.Lot.footprint);

            int currStories = app.selectedBuilding.Stories;
            int newStories = currStories + floors;
            float heightRatio = 1f;

            if (newStories < 0)
            {
                // We probably shouldn't allow negative floors
                return;
            }

            Vector3 scaleVector = app.selectedBuilding.EditBuildingTransform.Scale;

            if (currStories != 0)
            {
                heightRatio = (float)newStories / (float)currStories;

                scaleVector.Z = scaleVector.Z * heightRatio;
            }
            else
            {
                heightRatio = (float)newStories / (float)app.selectedBuilding.Lot.stories;

                scaleVector.Z = heightRatio * app.scale;
            }

            app.selectedBuilding.EditBuildingTransform.Scale = scaleVector;
            app.selectedBuilding.EditBuildingTransform.broadcast();
            app.selectedBuilding.Stories = newStories;

            Log.Write("editableBuildingTransformNode.Scale="
                + app.selectedBuilding.EditBuildingTransform.Scale.ToString() + "\n");

            GoblinXNA.UI.Notifier.AddMessage(
                app.selectedBuilding.Name + " now has "
                + app.selectedBuilding.Stories + " stories.");
        }
    }
}
