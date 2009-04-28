﻿using System;
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
    class Building : GeometryNode
    {
        Lot lot;
        TransformNode transformNode;
        EditableBuildingTransform editBuildingTransform;


        public Building(string address) : base(address)
        {
            System.Console.WriteLine(name);
        }


        public void setLot(Lot lot)
        {
            this.lot = lot;
        }


        public void setTransformNode(TransformNode transformNode)
        {
            this.transformNode = transformNode;
        }


        internal TransformNode getTransformNode()
        {
            return transformNode;
        }


        internal void setEditableTransform(EditableBuildingTransform editBuildingTransform)
        {
            this.editBuildingTransform = editBuildingTransform;
        }



        internal object getEditableTransformNode()
        {
            return this.editBuildingTransform;
        }


    }
}
