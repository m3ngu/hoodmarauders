using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


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
using Manhattanville.PieMenu;
using GoblinXNA.Device.Util;

namespace Manhattanville
{
    class DataRepresentation: TransformNode
    {
        GeometryNode boxNode1,boxNode2;
        TransformNode boxTransNode1,boxTransNode2;
        TexturedBox tb,tb1;
        float rotationAngle = 0;
        GraphicsDevice graphicsDevice;
        SpriteFont spriteFont;
        Color color;
        Material boxMaterial1;
        Material boxMaterial2;

        public DataRepresentation(GraphicsDevice graphicsDevice,SpriteFont spriteFont,Color color): base()
        {
            

           // boxNode1 = new GeometryNode("Box1");
            tb = new TexturedBox(1, 20, 20);
            boxNode1 = new GeometryNode("Box1");
            boxNode1.Model = new Model(tb.Mesh);
            boxNode1.Physics.Interactable = true;
            boxNode1.Physics.Collidable = false;
            boxNode1.Physics.MomentOfInertia =new Vector3(3,3,3) ;
            boxNode1.AddToPhysicsEngine = true;
            
            
            boxTransNode1 = new TransformNode();
            boxTransNode1.Translation = new Vector3(6, 0, 10);
            boxTransNode1.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
                MathHelper.ToRadians(60));

            boxMaterial1 = new Material();
            boxMaterial1.Diffuse = new Vector4(255, 255, 255, 0.5f);
            boxMaterial1.Specular = Color.White.ToVector4();
            boxMaterial1.SpecularPower = 10;
            boxNode1.Material = boxMaterial1;
            //String[] str = { buildingName};
            
            //for (int i = 0; i <str.Length; i++)
            //{
                
            //    boxMaterial1.Texture = Data.txt2Txt(graphicsDevice, "Address: \n"+str[i]+"\n" , 400, 400, spriteFont, color);
                
            //}
            
            /*TexturedBox tb = new TexturedBox(100, 20, 0.1f);
            GeometryNode textGeoNode = new GeometryNode("Text");
            textGeoNode.Model = new Model(tb.Mesh);
            textGeoNode.Material = boxMaterial1;*/
            

            //boxNode1.Material = textTransNode;
            //boxNode1.Material = boxMaterial1;

            tb1 = new TexturedBox(1, 20, 20);
            boxNode2 = new GeometryNode("Box2");
            //boxNode2.Model = new Box(1, 20, 20);
            boxNode2.Model = new Model(tb1.Mesh);
            
            boxNode2.AddToPhysicsEngine = true;
            //boxNode2.Physics.Shape = ShapeType.Box;
            
                       
            boxTransNode2 = new TransformNode();
            boxTransNode2.Translation = new Vector3(6, 0, 10);
            boxTransNode2.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
                MathHelper.ToRadians(150));
            boxMaterial2 = new Material();
            boxMaterial2.Diffuse = new Vector4(255, 255, 255, 0.5f);
            boxMaterial2.Specular = Color.White.ToVector4();
            boxMaterial2.SpecularPower = 10;
            boxNode2.Material = boxMaterial2;
            


            boxTransNode1.AddChild(boxNode1);
            //groundMarkerNode.AddChild(boxTransNode2);
            boxTransNode2.AddChild(boxNode2);
            this.AddChild(boxTransNode2);
            this.AddChild(boxTransNode1);
           
            
            // Create a collision pair and add a collision callback function that will be
            // called when the pair collides
            this.graphicsDevice = graphicsDevice;
            this.spriteFont = spriteFont;
            this.color = color;
        }

        //public void Update(double gameTime)
        //{
        //    rotationAngle += (float)gameTime*0.5f;            
        //}

        public void Update(double gameTime)
        {
            rotationAngle += (float)gameTime * 0.5f;
            Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), rotationAngle);
            Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), rotationAngle);
        }

        
        public void showData(Building selectedBuilding)
        {
            //String buildingName = selectedBuilding.Lot.name;
            //String buildingAddress = selectedBuilding.Lot.saleDate;
            boxMaterial1.Texture = Data.txt2Txt(graphicsDevice,
                "\n\n\n\nAddress:   " + selectedBuilding.Lot.name +
                "\n\nSale Date:   " + selectedBuilding.Lot.saleDate +
                "\n\nSale Price:   " + selectedBuilding.Lot.salePrice +
                "\n\nStories:   " + selectedBuilding.Lot.stories +
                "\n\nMaximum Floor Area:   " + selectedBuilding.Lot.maxFlrAreaRatio +
                "\n\nLot Area:   " + selectedBuilding.Lot.lotArea +
                "\n\nZoning District:   " + selectedBuilding.Lot.zoningDistrict,
                300, 300, spriteFont, color);
            
            boxMaterial2.Texture = Data.txt2Txt(graphicsDevice,
                 "\n\n\n\nActual Land:   " + selectedBuilding.Lot.actualLand +
                 "\n\nActual Total:   " + selectedBuilding.Lot.actualTotal +
                 "\n\nAir Rights:   " + selectedBuilding.Lot.airRights +
                 "\n\nBuilding Gross Area:   " + selectedBuilding.Lot.bldgGrossArea +
                 "\n\nBlock And Lot:   " + selectedBuilding.Lot.blockAndLot +
                 "\n\nBuilding:   " + selectedBuilding.Lot.building +
                 "\n\nBuilding Class:   " + selectedBuilding.Lot.buildingClass,
                 300, 300, spriteFont, color);
           

            


            //scene.RootNode.AddChild(groundMarkerNode);
            //groundMarkerNode.AddChild(boxTransNode1);
            
        }
    }
}
