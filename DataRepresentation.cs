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
        public GeometryNode boxNode1,boxNode2,boxNode3,boxNode4;
        public TransformNode boxTransNode1, boxTransNode2, boxTransNode3, boxTransNode4;
        TexturedBox tb,tb1,tb2,tb3;
        float rotationAngle = 0;
        GraphicsDevice graphicsDevice;
        SpriteFont spriteFont;
        Color color;
        Material boxMaterial1,boxMaterial2,boxMaterial3,boxMaterial4;
        internal static int height = 540;
        internal static int width = 400;
        
        //Scene scene;

        public DataRepresentation(GraphicsDevice graphicsDevice, SpriteFont spriteFont, Color color): base()
        {

            //scene = new Scene();
           // boxNode1 = new GeometryNode("Box1");
            tb = new TexturedBox(width/30, height/30, 0.1f);
            boxNode1 = new GeometryNode("Box1");
            boxNode1.Model = new Model(tb.Mesh);
            //boxNode1.Physics.Interactable = true;
            //boxNode1.Physics.Collidable = true;
            //boxNode1.Physics.MomentOfInertia =new Vector3(3,3,3) ;
            boxNode1.AddToPhysicsEngine = true;
            //Vector3 vector;
                      
            boxTransNode1 = new TransformNode();
            //boxTransNode1.Translation = new Vector3(6, 0, 10);
            //boxTransNode1.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
            //  MathHelper.ToRadians(90));
            //boxTransNode1.Translation = new Vector3(0, -10, 0);
            //boxTransNode1.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
            //MathHelper.ToRadians(60));

            boxMaterial1 = new Material();
            boxMaterial1.Diffuse = new Vector4(255, 255, 255, 1.0f);
            //boxMaterial1.Specular = new Vector4(255, 255, 255, 0.0f);
            //boxMaterial1.SpecularPower = 1000;
            boxNode1.Material = boxMaterial1;
            //vector = boxTransNode1.Translation;
            //Console.WriteLine("vectorrrrrrrrrrrrrrrrr" + vector);

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
            /*
            tb1 = new TexturedBox(1, 20, 20);
            boxNode2 = new GeometryNode("Box2");
            //boxNode2.Model = new Box(1, 20, 20);
            boxNode2.Model = new Model(tb1.Mesh);
            
            boxNode2.AddToPhysicsEngine = true;
            //boxNode2.Physics.Shape = ShapeType.Box;
            
                       
            boxTransNode2 = new TransformNode();
            boxTransNode2.Translation = new Vector3(6, 0, 10);
            boxTransNode2.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
            MathHelper.ToRadians(90));
            //boxTransNode2.Translation = new Vector3(6, 0, 10);
            //boxTransNode2.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
              //MathHelper.ToRadians(90));
            boxMaterial2 = new Material();
            boxMaterial2.Diffuse = new Vector4(255, 255, 255, 0.5f);
            boxMaterial2.Specular = Color.White.ToVector4();
            boxMaterial2.SpecularPower = 10;
            boxNode2.Material = boxMaterial2;
            */
            //boxNode3

            /*tb2 = new TexturedBox(1, 10, 10);
            boxNode3 = new GeometryNode("Box3");
            //boxNode2.Model = new Box(1, 20, 20);
            boxNode3.Model = new Model(tb2.Mesh);

            boxNode3.AddToPhysicsEngine = true;
            //boxNode2.Physics.Shape = ShapeType.Box; 


            boxTransNode3 = new TransformNode();
            boxTransNode3.Translation = new Vector3(0,0,10);
            boxTransNode3.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
              MathHelper.ToRadians(60));
            boxMaterial3 = new Material();
            boxMaterial3.Diffuse = new Vector4(255, 255, 255, 0.5f);
            boxMaterial3.Specular = Color.White.ToVector4();
            boxMaterial3.SpecularPower = 10;
            boxNode3.Material = boxMaterial3;
            
            //boxnode4

            tb3 = new TexturedBox(1, 10, 10);
            boxNode4 = new GeometryNode("Box4");
            //boxNode2.Model = new Box(1, 20, 20);
            boxNode4.Model = new Model(tb3.Mesh);

            boxNode4.AddToPhysicsEngine = true;
            //boxNode2.Physics.Shape = ShapeType.Box;


            boxTransNode4 = new TransformNode();
            boxTransNode4.Translation = new Vector3(0, 0, 10);
            boxTransNode4.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0),
                MathHelper.ToRadians(90));
            boxMaterial4 = new Material();
            boxMaterial4.Diffuse = new Vector4(255, 255, 255, 0.5f);
            boxMaterial4.Specular = Color.White.ToVector4();
            boxMaterial4.SpecularPower = 10;
            boxNode4.Material = boxMaterial4;*/


            boxTransNode1.AddChild(boxNode1);
            //groundMarkerNode.AddChild(boxTransNode2);
            //boxTransNode2.AddChild(boxNode2);
            //boxTransNode3.AddChild(boxNode3);
            //boxTransNode4.AddChild(boxNode4);
            this.AddChild(boxTransNode1);
            //this.AddChild(boxTransNode2);
            //this.AddChild(boxTransNode3);
            //this.AddChild(boxTransNode4);

            //((NewtonPhysics)scene.PhysicsEngine).ApplyAngularVelocity(boxNode1.Physics,
              //           Vector3.Up * -5.0f);
            
            // Create a collision pair and add a collision callback function that will be
            // called when the pair collides
            this.graphicsDevice = graphicsDevice;
            this.spriteFont = spriteFont;
            this.color = color;
        }

        public void Update(double gameTime)
        {
            rotationAngle += (float)gameTime*0.5f;
            //boxTransNode1.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), rotationAngle);
            //boxTransNode2.Rotation = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), rotationAngle);        
        }

        public void showData(Building selectedBuilding)
        {
            this.enabled = (selectedBuilding != null);
        }

        public void renderData(Building selectedBuilding)
        {

            if (selectedBuilding == null) return;

            if (selectedBuilding.DataChanged)
            {
                String price = "N/A";

                if (selectedBuilding.Lot.salePrice > 0)
                    price = "$ " + selectedBuilding.Lot.salePrice.ToString("#,###");

                selectedBuilding.RenderedDataText = Data.txt2Txt(graphicsDevice,
                    "\n   ___BUILDING______________________________\n" +
                    "\n   Address:   " + selectedBuilding.Lot.knownAs +
                    //"\n   Building:   " + selectedBuilding.Lot.building +
                    "\n   Building Class:   " + selectedBuilding.Lot.buildingClass +
                    "\n   Year Built:   " + selectedBuilding.Lot.yearBuilt +
                    "\n" +
                    "\n   ___LOT___________________________________\n" +
                    "\n   Block And Lot:   " + selectedBuilding.Lot.blockAndLot +
                    "\n   Lot Area:   " + selectedBuilding.Lot.lotArea.ToString("#,###") +
                    "\n   Lot Depth:   " + selectedBuilding.Lot.lotDepth.ToString("#,###") +
                    "\n   Lot Frontage:   " + selectedBuilding.Lot.lotFrontage.ToString("#,###") +
                    "\n" +
                    "\n   ___AIR_RIGHTS____________________________\n" +
                    "\n   Zoning District:   " + selectedBuilding.Lot.zoningDistrict +
                    "\n   Stories:   " + selectedBuilding.Stories +
                    //"\n   Floors:   " + selectedBuilding.Lot.floors +
                    "\n   Available Air Rights:   " + selectedBuilding.Lot.airRights.ToString("#,###") +
                    //"\n   Maximum Floor Area:   " + selectedBuilding.Lot.maxFlrAreaRatio +
                    //"\n   Building Gross Area:   " + selectedBuilding.Lot.bldgGrossArea +
                    "\n" +
                    "\n   ___COMMERCIAL___________________________\n" +
                    "\n   Sale Price:  " + price +
                    "\n   Sale Date:   " + selectedBuilding.Lot.saleDate
                    
                    /*
                    "\nCensus Tract:   " + selectedBuilding.Lot.censusTract +
                    "\nActual Land:   " + selectedBuilding.Lot.actualLand +
                    "\nActual Total:   " + selectedBuilding.Lot.actualTotal +
                    "\nResidential Units:   " + selectedBuilding.Lot.residentialUnits +
                    "\nCommercial Units:   " + selectedBuilding.Lot.commercialUnits +
                    "\nToxic Sites:   " + selectedBuilding.Lot.toxicSites
                    */
                    ,
                    width, height, spriteFont, color);
                
                selectedBuilding.DataChanged = false;
            }

            boxMaterial1.Texture = selectedBuilding.RenderedDataText;

            //String buildingName = selectedBuilding.Lot.name;
            //String buildingAddress = selectedBuilding.Lot.saleDate;
            /*boxMaterial1.Texture = Data.txt2Txt(graphicsDevice,
                "\n\n\nAddress:   " + selectedBuilding.Lot.name +
                "               Sale Date:   " + selectedBuilding.Lot.saleDate +
                "\n\nSale Price:   " + selectedBuilding.Lot.salePrice +
                "                   Year Built:   " + selectedBuilding.Lot.yearBuilt+
                "\n\nMaximum Floor Area:   " + selectedBuilding.Lot.maxFlrAreaRatio +
                "               Lot Area:   " + selectedBuilding.Lot.lotArea +
                "\n\nZoning District:   " + selectedBuilding.Lot.zoningDistrict+
                "               Census Tract:   " + selectedBuilding.Lot.censusTract+
                "\n\nCommercial Units:   " + selectedBuilding.Lot.commercialUnits+
                "               Maximum FloorArea:   " + selectedBuilding.Lot.maxFlrAreaRatio +
                "\n\nZoning District:   " + selectedBuilding.Lot.zoningDistrict +
                "               Stories:   " + selectedBuilding.Lot.stories ,
                300, 300, spriteFont, color);
            
            
            
            boxMaterial2.Texture = Data.txt2Txt(graphicsDevice,
                 "\n\n\n\nActual Land:   " + selectedBuilding.Lot.actualLand +
                 "              Actual Total:   " + selectedBuilding.Lot.actualTotal +
                 "\n\nAir Rights:   " + selectedBuilding.Lot.airRights +
                 "              Building Gross Area:   " + selectedBuilding.Lot.bldgGrossArea +
                 "\n\nBlock And Lot:   " + selectedBuilding.Lot.blockAndLot +
                 "              Building Class:   " + selectedBuilding.Lot.buildingClass +
                 "\n\nBuilding:   " + selectedBuilding.Lot.building +
                 "              Lot Depth:   " + selectedBuilding.Lot.lotDepth +
                 "\n\nFloors:   " + selectedBuilding.Lot.floors+
                 
                 "              Building:   " + selectedBuilding.Lot.lotFrontage +
                 "\n\nResidential Units:   " + selectedBuilding.Lot.residentialUnits +
                 "              Toxic Sites:   " + selectedBuilding.Lot.toxicSites ,
                 300, 300, spriteFont, color);*/

           /* boxMaterial3.Texture = Data.txt2Txt(graphicsDevice,
                "\n\n\n\nActual Land:   " + selectedBuilding.Lot.actualLand +
                //"              Actual Total:   " + selectedBuilding.Lot.actualTotal +
                "\n\nAir Rights:   " + selectedBuilding.Lot.airRights +
                //"              Building Gross Area:   " + selectedBuilding.Lot.bldgGrossArea +
                "\n\nBlock And Lot:   " + selectedBuilding.Lot.blockAndLot +
                //"              Building:   " + selectedBuilding.Lot.building +
                "\n\nBuilding Class:   " + selectedBuilding.Lot.buildingClass +
                //"              Floors:   " + selectedBuilding.Lot.floors +
                "\n\nLot Depth:   " + selectedBuilding.Lot.lotDepth +
                //"              Building:   " + selectedBuilding.Lot.lotFrontage +
                "\n\nResidential Units:   " + selectedBuilding.Lot.residentialUnits ,
                //"              Toxic Sites:   " + selectedBuilding.Lot.toxicSites,
                300, 300, spriteFont, color);

            boxMaterial4.Texture = Data.txt2Txt(graphicsDevice,
                "\n\n\n\nActual Land:   " + selectedBuilding.Lot.actualLand +
                //"              Actual Total:   " + selectedBuilding.Lot.actualTotal +
                "\n\nAir Rights:   " + selectedBuilding.Lot.airRights +
                //"              Building Gross Area:   " + selectedBuilding.Lot.bldgGrossArea +
                "\n\nBlock And Lot:   " + selectedBuilding.Lot.blockAndLot +
                //"              Building:   " + selectedBuilding.Lot.building +
                "\n\nBuilding Class:   " + selectedBuilding.Lot.buildingClass +
                //"              Floors:   " + selectedBuilding.Lot.floors +
                "\n\nLot Depth:   " + selectedBuilding.Lot.lotDepth +
                //"              Building:   " + selectedBuilding.Lot.lotFrontage +
                "\n\nResidential Units:   " + selectedBuilding.Lot.residentialUnits,
                //"              Toxic Sites:   " + selectedBuilding.Lot.toxicSites,
                300, 300, spriteFont, color);   */        


            //scene.RootNode.AddChild(groundMarkerNode);
            //groundMarkerNode.AddChild(boxTransNode1);
            
        }
    }
}
