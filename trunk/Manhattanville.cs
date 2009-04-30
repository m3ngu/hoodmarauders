//using SceneGraphDisplay;
//using System.Windows.Forms;

using System;
using System.Collections;
using System.Collections.Generic;
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
using Manhattanville.PieMenu;
using GoblinXNA.Device.Util;

namespace Manhattanville
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Manhattanville : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        //SGForm fs = null;
        Scene scene;
        MarkerNode groundMarkerNode;
        Dictionary<Building, Lot> lots;
        List<Building> buildings;
        Dictionary<Building, Building> editableBuildings;
        Building selectedBuilding;
        int closestIndex;
        Building selectedEditableBuilding;
        Lot selectedLot;
        MarkerNode toolMarkerNode;
        Tool tool;
        TransformNode parentTrans;
        EditableBuildingTransform editableBuildingTransformNode;
        PieMenu.PieMenu menu;
        PieMenuNode pieMenuRootNode;
        bool continousMode = false;
        SpriteFont font;

        float y_shift = -62;
        float x_shift = -28.0f;
        float factor = 135.0f / 1353;
        float scale = 0.00728f;

        int centerX, centerY;
        public Manhattanville()
        {
            graphics = new GraphicsDeviceManager(this);

            // Set vertical trace with the back buffer  
            //graphics.SynchronizeWithVerticalRetrace = false;

            // Use multi-sampling to smooth corners of objects  
            //graphics.PreferMultiSampling = true;

            // Set the update to run as fast as it can go or  
            // with a target elapsed time between updates  
            //IsFixedTimeStep = false;

            // Make the mouse appear  
            //IsMouseVisible = true;
            MouseInput.OnlyHandleInsideWindow = false;

            // Set back buffer resolution  
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

            // Make full screen  
            //graphics.ToggleFullScreen(); 

            menu = new PieMenu.PieMenu(this);

            Content.RootDirectory = "Content";
            //startSceneGraph();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 

        //private void startSceneGraph()
        //{
        //    fs = new SGForm(scene);
        //    MouseInput.MouseClickEvent += new HandleMouseClick(fs.SG_MouseClickHandler);
        //    fs.RunTool();
        //    //fs.Visible = true;
        //}

        protected override void Initialize()
        {
            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "manhattanville.xml");
            
            Settings.loadFromSettingsFile();

            // Initialize the scene graph
            scene = new Scene(this);

            // Use the newton physics engine to perform collision detection
            scene.PhysicsEngine = new NewtonPhysics();

            // Set up optical marker tracking
            // Note that we don't create our own camera when we use optical marker
            // tracking. It'll be created automatically
            SetupMarkerTracking();

            // Set up the lights used in the scene
            CreateLights();

            // Create 3D terrain on top of the map layout
            CreateTerrain(factor);

            // Load buildings
            //if (Settings.BuildingsDetailed)
            //    LoadDetailedBuildings(factor);
            //else
                LoadPlainBuildings(factor);

            foreach (Building b in buildings)
            {
                b.calcModelCoordinates();
                b.EditBuildingTransform.Translation += new Vector3(40f, -40f, (b.ModelHeight * scale) / 2f + 4f);
            }

            // EditArea
            GeometryNode editArea = new GeometryNode("EditArea");
            editArea.Model = new Model((new TexturedBox(80f,80f,4f)).Mesh);

            Material editMaterial = new Material();

            editMaterial.Diffuse = Color.White.ToVector4();
            editMaterial.Specular = Color.White.ToVector4();
            editMaterial.SpecularPower = 10;
            editMaterial.Texture = Content.Load<Texture2D>("Textures//Chessboard_wood");
            editArea.Material = editMaterial;

            TransformNode editTrans = new TransformNode(new Vector3(0f,40f,3f));
            editTrans.AddChild(editArea);
            groundMarkerNode.AddChild(editTrans);

            // Show Frames-Per-Second on the screen for debugging
            State.ShowFPS = true;
            State.ShowNotifications = Settings.ShowNotifications;
            GoblinXNA.UI.Notifier.FadeOutTime = Settings.FadeOutTime;
            GoblinXNA.UI.Notifier.Font = Content.Load<SpriteFont>("Fonts//NotifierSmall");

            // Add a mouse click handler for shooting a box model from the mouse location 
            MouseInput.MouseMoveEvent += new HandleMouseMove(MouseMoveHandler);
            KeyboardInput.KeyTypeEvent += new HandleKeyType(KeyTypeHandler);
            MouseInput.MousePressEvent += new HandleMousePress(MousePressHandler);

            centerX = Window.ClientBounds.Width / 2;
            centerY = Window.ClientBounds.Height / 2;
            MouseCenter();

            LoadMenu();

            font = Content.Load<SpriteFont>("Fonts//UIFont");

            Material mat = new Material();
            mat.Specular = Color.White.ToVector4();
            mat.Diffuse = Color.White.ToVector4();
            mat.SpecularPower = 10;
            mat.Texture = Data.txt2Txt(graphics.GraphicsDevice,"Hello World", 100, 20, font);

            //Hashtable lotInfo = lots[0].getInfoTable();
            //GoblinXNA.UI.Notifier.AddMessage(lots[0].name);
            //foreach (string key in lotInfo.Keys)
            //{
            //    GoblinXNA.UI.Notifier.AddMessage(key + ": " + lotInfo[key]);
            //}

            //mat.Texture.Save("test.jpg", ImageFileFormat.Jpg);

            TexturedBox tb = new TexturedBox(100, 20, 0.1f);
            GeometryNode textGeoNode = new GeometryNode("Text");
            textGeoNode.Model = new Model(tb.Mesh);
            textGeoNode.Material = mat;
            TransformNode textTransNode = new TransformNode(new Vector3(20, 20, 0));
            textTransNode.AddChild(textGeoNode);

            toolMarkerNode.AddChild(textTransNode);

            selectedBuilding = null;
            selectedLot = null;

            base.Initialize();
        }

        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(-1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.LightSources.Add(lightSource);

            scene.RootNode.AddChild(lightNode);
        }

        private void SetupMarkerTracking()
        {
            // Create our video capture device that uses DirectShow library. Note that 
            // the combinations of resolution and frame rate that are allowed depend on 
            // the particular video capture device. Thus, setting incorrect resolution 
            // and frame rate values may cause exceptions or simply be ignored, depending 
            // on the device driver.  The values set here will work for a Microsoft VX 6000, 
            // and many other webcams.
            DirectShowCapture captureDevice = new DirectShowCapture();
            //captureDevice.InitVideoCapture(0, FrameRate._30Hz, Resolution._640x480, 
            //    ImageFormat.R8G8B8_24, false);
//            captureDevice.InitVideoCapture(0, -1, FrameRate._30Hz, Resolution._640x480, false);
            captureDevice.InitVideoCapture(
                Settings.CameraID,
                -1, FrameRate._30Hz,
                Settings.Resolution,
                false);

            // Add this video capture device to the scene so that it can be used for
            // the marker tracker
            scene.AddVideoCaptureDevice(captureDevice);

            // Create a optical marker tracker that uses ARTag library
            ARTagTracker tracker = new ARTagTracker();
            // Set the configuration file to look for the marker specifications
            tracker.InitTracker(
                Settings.CameraFx,
                Settings.CameraFy,
                captureDevice.Width,
                captureDevice.Height,
                false,
                "manhattanville.cf");

            scene.MarkerTracker = tracker;

            // Create a marker node to track the ground marker arrays
            groundMarkerNode = new MarkerNode(scene.MarkerTracker, "ground");
            scene.RootNode.AddChild(groundMarkerNode);

            groundMarkerNode.Optimize = Settings.GroundOptimize;
            groundMarkerNode.MaxDropouts = Settings.GroundMaxDropouts;

            toolMarkerNode = new MarkerNode(scene.MarkerTracker, Settings.ToolTagName);
            scene.RootNode.AddChild(toolMarkerNode);

            toolMarkerNode.Smoother = new DESSmoother(Settings.ToolSmoother, Settings.ToolSmoother);
            toolMarkerNode.Optimize = Settings.ToolOptimize;
            toolMarkerNode.MaxDropouts = Settings.ToolMaxDropouts;

            tool = new Tool();
            tool.Marker = toolMarkerNode;
            toolMarkerNode.AddChild(tool);

            // Display the camera image in the background
            scene.ShowCameraImage = true;
        }

        private void CreateTerrain(float factor)
        {
            float y_gap = 120.0f / 6;
            float x_gap = 71.1f / 4;
            float tu_gap = 1.0f / 6;
            float tv_gap = 1.0f / 4;

            float[][] terrain_heights = new float[7][];
            for (int i = 0; i < 7; i++)
                terrain_heights[i] = new float[5];

            terrain_heights[0][0] = 28;
            terrain_heights[0][1] = 48;
            terrain_heights[0][2] = 57;
            terrain_heights[0][3] = 66;
            terrain_heights[0][4] = 68;
            terrain_heights[1][0] = 20;
            terrain_heights[1][1] = 29;
            terrain_heights[1][2] = 43;
            terrain_heights[1][3] = 53;
            terrain_heights[1][4] = 58;
            terrain_heights[2][0] = 16;
            terrain_heights[2][1] = 24;
            terrain_heights[2][2] = 32;
            terrain_heights[2][3] = 40;
            terrain_heights[2][4] = 46;
            terrain_heights[3][0] = 13;
            terrain_heights[3][1] = 18;
            terrain_heights[3][2] = 25;
            terrain_heights[3][3] = 29;
            terrain_heights[3][4] = 35;
            terrain_heights[4][0] = 10;
            terrain_heights[4][1] = 13;
            terrain_heights[4][2] = 17;
            terrain_heights[4][3] = 21;
            terrain_heights[4][4] = 26;
            terrain_heights[5][0] = 14;
            terrain_heights[5][1] = 15;
            terrain_heights[5][2] = 18;
            terrain_heights[5][3] = 23;
            terrain_heights[5][4] = 23;
            terrain_heights[6][0] = 44;
            terrain_heights[6][1] = 35;
            terrain_heights[6][2] = 32;
            terrain_heights[6][3] = 26;
            terrain_heights[6][4] = 24;

            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 5; j++)
                    terrain_heights[i][j] *= factor;

            PrimitiveMesh terrain = new PrimitiveMesh();

            VertexPositionNormalTexture[] verts = new VertexPositionNormalTexture[35];
            
            int index = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    index = i * 7 + j;

                    verts[index].Position = new Vector3(j * y_gap + y_shift, i * x_gap + x_shift, 
                        terrain_heights[j][i]);
                    verts[index].TextureCoordinate = new Vector2(j * tu_gap, 1 - i * tv_gap);
                    verts[index].Normal = Vector3.UnitZ;
                }
            }

            terrain.VertexBuffer = new VertexBuffer(graphics.GraphicsDevice,
                VertexPositionNormalTexture.SizeInBytes * 35, BufferUsage.None);
            terrain.SizeInBytes = VertexPositionNormalTexture.SizeInBytes;
            terrain.VertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice,
                VertexPositionNormalTexture.VertexElements);
            terrain.VertexBuffer.SetData(verts);
            terrain.NumberOfVertices = 35;


            short[] indices = new short[6 * 4 * 2 * 3];
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    index = i * 7 + j;

                    indices[count++] = (short)index;
                    indices[count++] = (short)(index + 7);
                    indices[count++] = (short)(index + 1);

                    indices[count++] = (short)(index + 7);
                    indices[count++] = (short)(index + 8);
                    indices[count++] = (short)(index + 1);
                }
            }

            terrain.IndexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(short), indices.Length,
                BufferUsage.WriteOnly);
            terrain.IndexBuffer.SetData(indices);

            terrain.PrimitiveType = PrimitiveType.TriangleList;
            terrain.NumberOfPrimitives = indices.Length / 3;

            Model terrainModel = new Model(terrain);

            GeometryNode terrainNode = new GeometryNode();
            terrainNode.Model = terrainModel;

            Material terrainMaterial = new Material();
            terrainMaterial.Diffuse = Color.White.ToVector4();
            terrainMaterial.Specular = Color.White.ToVector4();
            terrainMaterial.SpecularPower = 10;
            terrainMaterial.Texture = Content.Load<Texture2D>("Textures//Manhattanville");

            terrainNode.Material = terrainMaterial;

            TransformNode terrainTransNode = new TransformNode();
            terrainTransNode.Translation = new Vector3(7, 2.31f, 0);
            terrainTransNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
                MathHelper.PiOver2);

            groundMarkerNode.AddChild(terrainTransNode);
            terrainTransNode.AddChild(terrainNode);

            CreateSkirf(terrain_heights, terrainTransNode);
        }

        private void CreateSkirf(float[][] terrainHeights, TransformNode transNode)
        {
            float y_gap = 120.0f / 6;
            float x_gap = 71.1f / 4;

            PrimitiveMesh skirf = new PrimitiveMesh();

            VertexPositionNormal[] verts = new VertexPositionNormal[48];

            int index = 0;

            for (int i = 0; i < 7; i++)
            {
                verts[index].Position = new Vector3(i * y_gap + y_shift, x_shift, terrainHeights[i][0]);
                verts[index].Normal = Vector3.UnitY;
                index++;

                verts[index].Position = new Vector3(i * y_gap + y_shift, x_shift, 0);
                verts[index].Normal = Vector3.UnitY;
                index++;

                verts[index].Position = new Vector3((6 - i) * y_gap + y_shift, 71.1f + x_shift, 
                    terrainHeights[6-i][4]);
                verts[index].Normal = -Vector3.UnitY;
                index++;

                verts[index].Position = new Vector3((6 - i) * y_gap + y_shift, 71.1f + x_shift, 0);
                verts[index].Normal = -Vector3.UnitY;
                index++;
            }

            for (int i = 0; i < 5; i++)
            {
                verts[index].Position = new Vector3(y_shift, (4 - i) * x_gap + x_shift, terrainHeights[0][(4 - i)]);
                verts[index].Normal = Vector3.UnitX;
                index++;

                verts[index].Position = new Vector3(y_shift, (4 - i) * x_gap + x_shift, 0);
                verts[index].Normal = Vector3.UnitX;
                index++;

                verts[index].Position = new Vector3(120.0f + y_shift, i * x_gap + x_shift, terrainHeights[6][i]);
                verts[index].Normal = -Vector3.UnitX;
                index++;

                verts[index].Position = new Vector3(120.0f + y_shift, i * x_gap + x_shift, 0);
                verts[index].Normal = -Vector3.UnitX;
                index++;
            }

            skirf.VertexBuffer = new VertexBuffer(graphics.GraphicsDevice,
                VertexPositionNormal.SizeInBytes * 48, BufferUsage.None);
            skirf.SizeInBytes = VertexPositionNormal.SizeInBytes;
            skirf.VertexDeclaration = new VertexDeclaration(graphics.GraphicsDevice,
                VertexPositionNormal.VertexElements);
            skirf.VertexBuffer.SetData(verts);
            skirf.NumberOfVertices = 48;


            short[] indices = new short[20 * 2 * 3];
            index = 0;

            for (int i = 0; i < 11; i++)
            {
                if (i != 6)
                {
                    indices[index++] = (short)(i * 4 + 1);
                    indices[index++] = (short)(i * 4);
                    indices[index++] = (short)((i + 1) * 4);

                    indices[index++] = (short)(i * 4 + 1);
                    indices[index++] = (short)((i + 1) * 4);
                    indices[index++] = (short)((i + 1) * 4 + 1);

                    indices[index++] = (short)(i * 4 + 3);
                    indices[index++] = (short)(i * 4 + 2);
                    indices[index++] = (short)((i + 1) * 4 + 2);

                    indices[index++] = (short)(i * 4 + 3);
                    indices[index++] = (short)((i + 1) * 4 + 2);
                    indices[index++] = (short)((i + 1) * 4 + 3);
                }
            }

            skirf.IndexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(short), indices.Length,
                BufferUsage.WriteOnly);
            skirf.IndexBuffer.SetData(indices);

            skirf.PrimitiveType = PrimitiveType.TriangleList;
            skirf.NumberOfPrimitives = indices.Length / 3;

            Model skirfModel = new Model(skirf);

            GeometryNode skirfNode = new GeometryNode();
            skirfNode.Model = skirfModel;

            Material skirfMaterial = new Material();
            skirfMaterial.Diffuse = Color.White.ToVector4();
            skirfMaterial.Specular = Color.White.ToVector4();
            skirfMaterial.SpecularPower = 10;

            skirfNode.Material = skirfMaterial;

            transNode.AddChild(skirfNode);
        }

        private void LoadPlainBuildings(float factor)
        {
            String filename = "buildings_plain.csv";
            if (Settings.BuildingsSubset)
                filename = "buildings_plain_subset.csv";

            FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(file);

            lots = new Dictionary<Building, Lot>();
            buildings = new List<Building>();
            editableBuildings = new Dictionary<Building, Building>();
            ModelLoader loader = new ModelLoader();

            float zRot, x, y, z;
            String[] chunks;
            char[] seps = { ',' };

            String s = "";
            try
            {
                // Skip the first line which has column names
                sr.ReadLine();

                parentTrans = new TransformNode();
                parentTrans.Translation = new Vector3(-12.5f, -15.69f, 0);
                parentTrans.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180);
                groundMarkerNode.AddChild(parentTrans);

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();

                    if (s.Length > 0)
                    {
                        //chunks = s.Split(seps);
                        chunks = s.Split(seps, System.StringSplitOptions.None);
                        //Console.WriteLine("size of chunks: " + chunks.Length);

                        Lot lot = new Lot(chunks);
                        Building building = new Building(chunks[0]);
                        Building editableBuilding = new Building(chunks[0] + "_edit");
                        lot.addBuilding(building);
                        building.Lot = lot;
                        building.Model = (Model)loader.Load("", "Plain/" + chunks[0]);
                        building.AddToPhysicsEngine = true;
                        building.Physics.Shape = ShapeType.Box;

                        editableBuilding.Model = (Model)loader.Load("", "Plain/" + chunks[0]);
                        editableBuilding.AddToPhysicsEngine = true;
                        editableBuilding.Physics.Shape = ShapeType.Box;
						editableBuilding.Model.OffsetToOrigin = true;

                        lot.readInfo(chunks);
                        //System.Console.WriteLine(lot.floors);

                        lots.Add(building, lot);
                        buildings.Add(building);

                        editableBuildings.Add(building,editableBuilding);

                        zRot = (float)Double.Parse(chunks[1]);
                        x = (float)Double.Parse(chunks[2]);
                        y = (float)Double.Parse(chunks[3]);
                        z = (float)Double.Parse(chunks[4]);

                        TransformNode transNode = new TransformNode();
                        transNode.Translation = new Vector3(x, y, z * factor);
                        transNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
                            (float)(zRot * Math.PI / 180)) * Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.PiOver2);
                        transNode.Scale = Vector3.One * scale;

                        editableBuildingTransformNode = new EditableBuildingTransform();
                        //editableBuildingTransformNode.Translation = new Vector3(0, 0, 0);//-12.5f, -15.69f, 0);
                        //editableBuildingTransformNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0); ;// Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180);
                        editableBuildingTransformNode.Translation = new Vector3(x, y, z * factor);
                        editableBuildingTransformNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
                            (float)(zRot * Math.PI / 180)) * Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.PiOver2);
                        editableBuildingTransformNode.Scale = Vector3.One * scale * new Vector3(3);

                        Material buildingMaterial = new Material();
                        buildingMaterial.Diffuse = Color.White.ToVector4();
                        buildingMaterial.Specular = Color.White.ToVector4();
                        buildingMaterial.SpecularPower = 10;

                        building.Material = buildingMaterial;

                        Material editableBuildingMaterial = new Material();
                        editableBuildingMaterial.Diffuse = Color.Red.ToVector4();
                        editableBuildingMaterial.Specular = Color.White.ToVector4();
                        editableBuildingMaterial.SpecularPower = 10;

                        editableBuilding.Material = editableBuildingMaterial;

                        building.TransformNode = transNode;
                        building.EditBuildingTransform = editableBuildingTransformNode;
                        editableBuilding.TransformNode = editableBuildingTransformNode;

                        parentTrans.AddChild(transNode);
                        //parentTrans.AddChild(editableBuildingTransformNode);
                        lot.transformNode = transNode;

                        transNode.AddChild(building);
                        editableBuildingTransformNode.AddChild(editableBuilding);
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(filename + " has wrong format: " + s);
                Console.WriteLine(exp.Message);
            }

            sr.Close();
            file.Close();
        }

        //private void LoadDetailedBuildings(float factor)
        //{
        //    FileStream file = new FileStream("buildings_detailed.csv", FileMode.Open, FileAccess.Read);
        //    StreamReader sr = new StreamReader(file);

        //    lots = new List<Lot>();
        //    buildings = new List<Building>();
        //    ModelLoader loader = new ModelLoader();

        //    float scale = 0.0073f;
        //    float zRot, x, y, z;
        //    String[] chunks;
        //    char[] seps = { ',' };

        //    String s = "";
        //    try
        //    {
        //        // Skip the first line which has column names
        //        sr.ReadLine();

        //        parentTrans = new TransformNode();
        //        parentTrans.Scale = Vector3.One * scale;
        //        parentTrans.Translation = new Vector3(-33.5f, -54.25f, 0);
        //        parentTrans.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.PiOver2);
        //        groundMarkerNode.AddChild(parentTrans);

        //        while (!sr.EndOfStream)
        //        {
        //            s = sr.ReadLine();

        //            if (s.Length > 0)
        //            {
        //                chunks = s.Split(seps);

        //                Building building = new Building(chunks[0]);
        //                building.Model = (Model)loader.Load("", "Detailed/" + chunks[0]);
        //                building.AddToPhysicsEngine = true;
        //                building.Physics.Shape = ShapeType.Box;

        //                buildings.Add(building);

        //                zRot = (float)Double.Parse(chunks[1]);
        //                x = (float)Double.Parse(chunks[2]);
        //                y = (float)Double.Parse(chunks[3]);
        //                z = (float)Double.Parse(chunks[4]);

        //                TransformNode transNode = new TransformNode();
        //                transNode.Translation = new Vector3(x, y, z * factor);
        //                transNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
        //                    (float)(zRot * Math.PI / 180)) * Quaternion.CreateFromAxisAngle(Vector3.UnitX,
        //                    MathHelper.PiOver2);

        //                Material buildingMaterial = new Material();
        //                buildingMaterial.Diffuse = Color.White.ToVector4();
        //                buildingMaterial.Specular = Color.White.ToVector4();
        //                buildingMaterial.SpecularPower = 10;

        //                building.Material = buildingMaterial;

        //                parentTrans.AddChild(transNode);
        //                transNode.AddChild(building);
        //            }
        //        }
        //    }
        //    catch (Exception exp)
        //    {
        //        Console.WriteLine("buildings.csv has wrong format: " + s);
        //        Console.WriteLine(exp.Message);
        //    }

        //    sr.Close();
        //    file.Close();
        //}

        private void LoadMenu()
        {
            pieMenuRootNode = new PieMenuNode();
            PieMenuNode parent, child;

            parent = new PieMenuNode("Node 1", this.Content.Load<Texture2D>("Icons\\merge"), null);
            pieMenuRootNode.Add(parent);

            child = new PieMenuNode("Node 1.1", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 1.2", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 1.3", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            parent = new PieMenuNode("Node 2", this.Content.Load<Texture2D>("Icons\\height"), null);
            pieMenuRootNode.Add(parent);

            child = new PieMenuNode("Node 2.1", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 2.2", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 2.3", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 2.4", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 2.5", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 2.6", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            parent = new PieMenuNode("Node 3", this.Content.Load<Texture2D>("Icons\\footprint"), null);
            pieMenuRootNode.Add(parent);

            child = new PieMenuNode("Node 3.1", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            child = new PieMenuNode("Node 3.2", this.Content.Load<Texture2D>("Icons\\paint"), new SimpleDelegate(MenuAction));
            parent.Add(child);

            parent = new PieMenuNode("Node 4", this.Content.Load<Texture2D>("Icons\\selectArrow"), new SimpleDelegate(getClosestBuilding));
            pieMenuRootNode.Add(parent);


        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            //UpdateMouse();
            if (continousMode) getClosestBuilding(null);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected void MouseCenter()
        {
            Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
        }

        protected void UpdateMouse(Vector2 selectionVector)
        {
            //GoblinXNA.UI.Notifier.AddMessage("Mouse Update!");

            menu.HandleInput(selectionVector);
            MouseCenter();

            //Console.WriteLine("x=" + Mouse.GetState().X + " y=" + Mouse.GetState().Y);
            //Console.WriteLine("x=" + Window.ClientBounds.Width + " y=" + Window.ClientBounds.Height);
        }

        private void MouseMoveHandler(Point mouseLocation)
        {
            int xDel = mouseLocation.X - centerX;
            int yDel = mouseLocation.Y - centerY;

            //GoblinXNA.UI.Notifier.AddMessage("x=" + xDel + " y=" + yDel);

            if (new Vector2((float)xDel, (float)yDel).Length() > 100.0f)
            {
                UpdateMouse(new Vector2(xDel, yDel));
            }
        }

        private void KeyTypeHandler(Keys key, KeyModifier modifier)
        {
            if (key == Keys.Q)
            {
                this.Exit();
            }

            if (key == Keys.S)
            {
                Utilities.SaveScreenShot(graphics.GraphicsDevice, "ScreenShot.jpg");
            }

            if (key == Keys.Space)
            {
                //MousePressHandler(MouseInput.LeftButton, new Point(centerX, centerY));
                getClosestBuilding(null);
            }

            if (key == Keys.Escape)
            {
                MousePressHandler(MouseInput.RightButton, new Point(centerX, centerY));
            }

            if (key == Keys.C)
            {
                continousMode = !continousMode;
                GoblinXNA.UI.Notifier.AddMessage("continousMode=" + continousMode);
            }

            if (key == Keys.Up)
            {
                if ( (selectedBuilding != null) && (selectedEditableBuilding != null) )
                {
                    addFloor(1);
                }
            }

            if (key == Keys.Down)
            {
                if ((selectedBuilding != null) && (selectedEditableBuilding != null))
                {
                    addFloor(-1);
                }
            }

            if (key == Keys.Tab)
            {
                Building b;
                try
                {
                    b = buildings[buildings.IndexOf(selectedBuilding) + 1];
                }
                catch
                {
                    b = buildings[0];
                }
                
                selectBuilding(b);
            }
        }

        private void MousePressHandler(int button, Point mouseLocation)
        {
            if (button == MouseInput.LeftButton)
            {
                //GoblinXNA.UI.Notifier.AddMessage("Left");
                if (!menu.Visible)
                {
                    menu.Show(pieMenuRootNode, new Vector2((float)centerX, (float)centerY));
                }
            }
            else if (button == MouseInput.RightButton)
            {
                //GoblinXNA.UI.Notifier.AddMessage("Right");

                menu.Back();
            }
        }

        public void MenuAction(Object sender)
        {
            PieMenuNode sndr = (PieMenuNode)sender;
            
            GoblinXNA.UI.Notifier.AddMessage(sndr.Text + " Selected!");
        }

        public void getClosestBuilding(Object sender)
        {
            if (!tool.Marker.MarkerFound) {
                if (!continousMode) GoblinXNA.UI.Notifier.AddMessage("Tool not found!");
                return;
            }

            //GoblinXNA.UI.Notifier.AddMessage("tool=" + tool.Marker.WorldTransformation.Translation.ToString());
            //GoblinXNA.UI.Notifier.AddMessage("ground=" + groundMarkerNode.WorldTransformation.Translation.ToString());

            float minDis = float.MaxValue;
            Building closestBuilding = null;

            int i = 0;
            foreach (Building b in buildings)
            {
                float dis = (b.getBaseWorld() - tool.Marker.WorldTransformation.Translation).Length();

                if (dis < minDis)
                {
                    minDis = dis;
                    closestBuilding = b;
                    closestIndex = i;
                }

                i++;

                /*
                            GoblinXNA.UI.Notifier.AddMessage(b.Name
                                + ", marker=" + b.MarkerTransform.Translation.ToString()
                                + ", modelMidPoint=" + modelMidPoint.ToString()
                                + ", combined=" + Vector3.Transform(modelMidPoint, m).ToString()
                                + ", dis=" + dis);
                            */

            }

            if (closestBuilding != null)
            {
                selectBuilding(closestBuilding);
            }

        }

        private void selectBuilding(Building b)
        {
            if (selectedBuilding != null)
            {
                selectedBuilding.Material.Diffuse = Color.White.ToVector4();
                //if (selectedEditableBuilding != null)
                //{
                parentTrans.RemoveChild(selectedBuilding.EditBuildingTransform);
                //}
                //selectedBuilding.setEditableTransform(null);
            }

            selectedBuilding = b;
            selectedEditableBuilding = editableBuildings[selectedBuilding];
            selectedLot = b.Lot;

            selectedBuilding.Material.Diffuse = Color.Red.ToVector4();
            parentTrans.AddChild(selectedBuilding.EditBuildingTransform);
            
            if (!continousMode) GoblinXNA.UI.Notifier.AddMessage(selectedBuilding.Name);
           
        }

        private void addFloor(int floors)
        {
            // TODO: We should probably convert numeric data to numeric variables
            // at load time
            int currStories = selectedBuilding.Stories;
            int newStories = currStories + floors;
            float heightRatio = 1f;

            if (newStories < 0)
            {
                // We probably shouldn't allow negative floors
                return;
            }

            editableBuildingTransformNode = (EditableBuildingTransform)(selectedBuilding.EditBuildingTransform);
            Vector3 scaleVector = editableBuildingTransformNode.Scale;

            if (currStories != 0)
            {
                heightRatio = (float)newStories / (float)currStories;

                scaleVector.Z = scaleVector.Z * heightRatio;
            }
            else
            {
                heightRatio = (float)newStories / float.Parse(selectedBuilding.Lot.stories);

                scaleVector.Z = heightRatio * scale;
            }

            editableBuildingTransformNode.Scale = scaleVector;

            selectedBuilding.Stories = newStories;

            Log.Write("editableBuildingTransformNode.Scale="
                + editableBuildingTransformNode.Scale.ToString() + "\n");

            GoblinXNA.UI.Notifier.AddMessage(
                selectedBuilding.Name + " now has "
                + selectedBuilding.Stories + " stories.");
        }

    }
}
