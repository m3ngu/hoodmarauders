using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Generic;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;

using GoblinXNA.UI.UI2D;
using GoblinXNA.UI.Events;

using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
namespace Manhattanville
{
    class EditableBuildingTransform : TransformNode
    {

        TransformNode originalTransform;

        public EditableBuildingTransform()
            : base()
        {
        }

        public EditableBuildingTransform(String name, Vector3 translation, Quaternion rotation, Vector3 scaling)
            : base(name, translation, rotation, scaling)
        {
        }

        public void setOriginalTransform(TransformNode original)
        {
            this.originalTransform = original;
        }

        public void update() {
        }

        internal void mimic(TransformNode original)
        {
            this.originalTransform = original;
            this.Translation = new Vector3(0, 0, 3);
            this.Rotation = original.Rotation;
            this.Scale = original.Scale * new Vector3(3, 3, 3);
        }


        internal object getOriginalTransform()
        {
            return originalTransform;
        }
    }
}
