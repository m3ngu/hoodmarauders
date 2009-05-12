//using SceneGraphDisplay;
//using System.Windows.Forms;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using System.Windows.Forms;

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
using SceneGraphDisplay;
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
using GoblinXNA.Device.iWear;
using GoblinXNA.Device;

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
        MarkerNode bigOne;
        Dictionary<Building, Lot> lots;
        List<Building> buildings;
        Dictionary<Building, Building> editableBuildings;
        internal Building selectedBuilding { get; set; }
        int closestIndex;
        Building selectedEditableBuilding;
        Lot selectedLot;
        MarkerNode toolMarkerNode;
        Tool tool;
        TransformNode parentTrans, parentTransEditable, handleTrans, editTrans;
        TransformNode parentTransBigOne, bigOneRotations, bigOneTranslations;
        //AirRightsNode airRightsNode;
        //AirRightsTransform airRightsTransformNode;
        AirRightsGraph airRightsGraph;
        DataRepresentation dataRepresentation;
        Color color;

        PieMenu.PieMenu menu;
        
        SpriteFont font;

        iWearTracker iTracker;

        List<Handle> handles = new List<Handle>(Enum.GetNames(typeof(Handle.Location)).Length);
        internal Handle selectedHandle { get; set; }

        float y_shift = -62;
        float x_shift = -28.0f;
        float factor = 135.0f / 1353;
        internal float scale = 0.00728f;

        int centerX, centerY;
        SoundEffect hum;
        String humName = "hum";
        internal SoundEffect pop;
        String popName = "pop";
        //SoundEffect whooshup;
        //String whooshupName = "whooshup";
        //SoundEffect whooshdown;
        //String whooshdownName = "whooshdown";

        int votesForChangingSelectedBuilding = 0;
        double cumulativeTime = 0;
        double lastMenuSelectionTime = 0;

        bool flag=true;
        float angle=0;

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
            MouseInput.Instance.OnlyHandleInsideWindow = false;

            // Set back buffer resolution  
            //graphics.PreferredBackBufferWidth = 1280;
            //graphics.PreferredBackBufferHeight = 720;

            // Make full screen  
            //graphics.ToggleFullScreen(); 

            menu = new PieMenu.PieMenu(this);

            Content.RootDirectory = "Content";
            //startSceneGraph();

            ContentManager contentManager = new ContentManager(this.Services, @"Content\Sounds\");
            hum = contentManager.Load<SoundEffect>(humName);
            pop = contentManager.Load<SoundEffect>(popName);
            //whooshup = contentManager.Load<SoundEffect>(whooshupName);
            //whooshdown = contentManager.Load<SoundEffect>(whooshdownName);
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
        //    MouseInput.Instance.MouseClickEvent += new HandleMouseClick(fs.SG_MouseClickHandler);
        //    fs.RunTool();
        //    fs.Visible = true;
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
            }

            initializeHandles();

            // EditArea
            GeometryNode editArea = new GeometryNode("EditArea");
            editArea.Model = new Box(80f, 80f, 1f);

            Material editMaterial = new Material();
            editMaterial.Diffuse = Color.DarkSlateBlue.ToVector4();//new Vector4(72, 61, 139, 1f);//
            editMaterial.Specular = Color.White.ToVector4();
            editMaterial.SpecularPower = 3f;
            editArea.Material = editMaterial;

            editTrans = new TransformNode(new Vector3(0f, 40f, 5f));
            editTrans.AddChild(editArea);
            groundMarkerNode.AddChild(editTrans);

            //THE CHESSBOARD IDEA
            //editArea.Model = new Model((new TexturedBox(80f,80f,4f)).Mesh);

            //Material editMaterial = new Material();

            //editMaterial.Diffuse = Color.White.ToVector4();
            //editMaterial.Specular = Color.White.ToVector4();
            //editMaterial.SpecularPower = 10;
            //editMaterial.Texture = Content.Load<Texture2D>("Textures//Chessboard_wood");
            //editArea.Material = editMaterial;

            //TransformNode editTrans = new TransformNode(new Vector3(0f,40f,3f));
            //editTrans.AddChild(editArea);
            //groundMarkerNode.AddChild(editTrans);

            //CreateAirRightsGraph();

            // Show Frames-Per-Second on the screen for debugging
            State.ShowFPS = true;
            State.ShowNotifications = Settings.ShowNotifications;
            GoblinXNA.UI.Notifier.FadeOutTime = Settings.FadeOutTime;
            GoblinXNA.UI.Notifier.Font = Content.Load<SpriteFont>("Fonts//NotifierSmall");

            // Add a mouse click handler for shooting a box model from the mouse location 
            MouseInput.Instance.MouseMoveEvent += new HandleMouseMove(MouseMoveHandler);
            KeyboardInput.Instance.KeyTypeEvent += new HandleKeyType(KeyTypeHandler);
            MouseInput.Instance.MousePressEvent += new HandleMousePress(MousePressHandler);

            centerX = Window.ClientBounds.Width / 2;
            centerY = Window.ClientBounds.Height / 2;
            MouseCenter();

            font = Content.Load<SpriteFont>("Fonts//UIFont");

            color = new Color(255, 255, 0, 50000); 
            /*
            Material mat = new Material();
            mat.Specular = Color.White.ToVector4();
            mat.Diffuse = Color.White.ToVector4();
            mat.SpecularPower = 10;
            mat.Texture = Data.txt2Txt(graphics.GraphicsDevice,"Hello World", 100, 20, font, color);

            //mat.Texture.Save("test.jpg", ImageFileFormat.Jpg);

            TexturedBox tb = new TexturedBox(100, 20, 0.1f);
            GeometryNode textGeoNode = new GeometryNode("Text");
            textGeoNode.Model = new Model(tb.Mesh);
            textGeoNode.Material = mat;
            TransformNode textTransNode = new TransformNode(new Vector3(20, 20, 0));
            textTransNode.AddChild(textGeoNode);

            toolMarkerNode.AddChild(textTransNode);
            */
            selectedBuilding = null;
            selectedLot = null;

            AppStateMgr.initialize(this, graphics);
            AppStateMgr.enter(AppState.Browse);

            ModificationManager.initialize(this, graphics);

            bigOneTranslations.AddChild(buildings[3].RealBuildingTransform);
            //bigOneTranslations.Translation = Vector3.UnitZ * -50;

            loadData();
            base.Initialize();
        }

        private void loadData()
        {


            dataRepresentation = new
DataRepresentation(graphics.GraphicsDevice, font, color);
            //dataRepresentation.Model = new Box(1,20,20);

            dataRepresentation.Translation = new Vector3(-10, 40, -20);
            //dataRepresentation.Rotation =
            Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(90));
            dataRepresentation.Scale = new Vector3(3, 3, 3);
            groundMarkerNode.AddChild(dataRepresentation);
            dataRepresentation.Enabled = false;

            ((NewtonPhysics)scene.PhysicsEngine).ApplyAngularVelocity(dataRepresentation.boxNode1.Physics,
                                     new Vector3(1, 0, 0));

        }
        //private void CreateAirRightsGraph()
        //{
        //    throw new NotImplementedException();
        //    airRightsGraphTransform = new AirRightsGraph();

        //}

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
                FrameRate._30Hz,
                Settings.Resolution,
                ImageFormat.R8G8B8_24, false);

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

            // Create a marker node to track the ground marker arrays
            bigOne = new MarkerNode(scene.MarkerTracker, "bigone");
            scene.RootNode.AddChild(bigOne);

            bigOne.Optimize = true; // Settings.BigOneOptimize;
            bigOne.MaxDropouts = Settings.BigOneMaxDropouts;

            //bigOne.AddChild(Utilities.debugSphere(1000));

            // Display the camera image in the background
            scene.ShowCameraImage = true;

            //scene.CameraNode.BoundingFrustum.Far = new Plane(-Vector3.UnitZ, -5000);
            //Matrix proj = Matrix.CreatePerspectiveFieldOfView(scene.CameraNode.Camera.FieldOfViewY,
            //            scene.CameraNode.Camera.AspectRatio, 1.0f, 10000.0f);
            //scene.CameraNode.BoundingFrustum = new BoundingFrustum(scene.CameraNode.Camera.View * proj);
            
            scene.CameraNode.Camera.ZFarPlane = 100000000.0f;
        }

        private void SetupIWear()
        {
            // Get an instance of iWearTracker
            iTracker = iWearTracker.Instance;
            // We need to initialize it before adding it to the InputMapper class
            iTracker.Initialize();
            // If not stereo, then we need to set the iWear VR920 to mono mode (by default, it's
            // stereo mode if stereo is available)
            //if (!stereoMode)
            //    iTracker.EnableStereo = false;
            // Add this iWearTracker to the InputMapper class for automatic update and disposal
            InputMapper.Instance.Add6DOFInputDevice(iTracker);
            // Re-enumerate all of the input devices so that the newly added device can be found
            InputMapper.Instance.Reenumerate();
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

                //////// BUILD PARENT TRANSFORMS CONTAINING ALL MAP AND GRAPH NODES, RESPECTIVELY

                //BuildingTransform editableBuildingTransformNode;
                //BuildingTransform realBuildingTransformNode;

                parentTrans = new TransformNode();
                parentTrans.Translation = new Vector3(-12.5f, -15.69f, 0);
                parentTrans.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180);

                parentTransBigOne = new TransformNode();
                //parentTransBigOne.Translation = new Vector3(-12.5f, -15.69f, 0);
                parentTransBigOne.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180);

                parentTransEditable = new TransformNode();
                parentTransEditable.Translation = new Vector3(0f, 40f, 5f);
                parentTransEditable.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 119 * MathHelper.Pi / 180);
                //parentTransEditable.Scale = new Vector3(Settings.EditableScale);

                handleTrans = new TransformNode();
                handleTrans.Translation = new Vector3(0f, 40f, 5f);
                handleTrans.Scale = new Vector3(Settings.EditableScale);

                airRightsGraph = new AirRightsGraph();
                groundMarkerNode.AddChild(airRightsGraph);
                //parentTrans.AddChild(airRightsGraph);

                groundMarkerNode.AddChild(parentTrans);
                groundMarkerNode.AddChild(parentTransEditable);
                groundMarkerNode.AddChild(handleTrans);

                /*************************************/
                /*****  BIG ONE TRANSFORMATIONS  *****/
                /*************************************/

                //bigOneRotations = new TransformNode();
                bigOneTranslations = new TransformNode();

                //bigOneRotations.AddChild(parentTransBigOne);   // marker <- translations <- rotations <- parent
                //bigOneTranslations.AddChild(bigOneRotations);
                bigOne.AddChild(bigOneTranslations);

                //bigOneTranslations.Translation = new Vector3(-13156, 93713, -58921);
                //bigOneTranslations.Scale = new Vector3(200, 200, 200);

                //bigOneRotations.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(90))
                //    * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(90));

                while (!sr.EndOfStream)
                {
                    s = sr.ReadLine();

                    if (s.Length > 0)
                    {
                        //chunks = s.Split(seps);
                        chunks = s.Split(seps, System.StringSplitOptions.None);
                        //Console.WriteLine("size of chunks: " + chunks.Length);

                        /////////////// BUILD BUILDINGS AND GRAPHICAL REPRESENTATION
                        
                        String address = chunks[0];
                        Console.WriteLine(address);
                        Lot lot = new Lot(chunks);
                        Building building = new Building( address );
                        Building editableBuilding = new Building( address + "_edit" );
                        Building realBuilding = new Building( "Real_" + address);
                        lot.addBuilding(building);
                        building.Lot = lot;
                        building.Model = (Model)loader.Load("", "Plain/" + address);
                        building.AddToPhysicsEngine = true;
                        building.Physics.Shape = ShapeType.Box;

                        editableBuilding.Model = (Model)loader.Load("", "Plain/" + address);
                        editableBuilding.AddToPhysicsEngine = true;
                        editableBuilding.Physics.Shape = ShapeType.Box;
						editableBuilding.Model.OffsetToOrigin = true;

                        //realBuilding.Model = (Model)loader.Load("", "Plain/" + address);
                        realBuilding.Model = new Box(100);
                        //realBuilding.AddToPhysicsEngine = true;
                        //realBuilding.Physics.Shape = ShapeType.Box;
                        //realBuilding.Model.OffsetToOrigin = true;

                        AirRightsNode airRightsNode = new AirRightsNode(address + "_air_rights", building);
                            
                        //realBuilding.Model = (Model)loader.Load("", "Plain/" + address);
                        //realBuilding.AddToPhysicsEngine = true;
                        //realBuilding.Physics.Shape = ShapeType.Box;
                        //realBuilding.Model.OffsetToOrigin = true;

                        lots.Add(building, lot);
                        buildings.Add(building);
                        editableBuildings.Add(building,editableBuilding);

                        zRot = (float)Double.Parse(chunks[1]);
                        x = (float)Double.Parse(chunks[2]);
                        y = (float)Double.Parse(chunks[3]);
                        z = (float)Double.Parse(chunks[4]);

                        float graphNodeXOffset = (float)Double.Parse(chunks[29]);
                        float graphNodeYOffset = (float)Double.Parse(chunks[30]);
                        //Console.WriteLine(" graphNodeXOffset = " + graphNodeXOffset);
                        //Console.WriteLine(" graphNodeYOffset = " + graphNodeYOffset);

                        ///////////// BUILD TRANSFORM NODES

                        BuildingTransform transNode = new BuildingTransform(building,(1.0f/Settings.EditableScale));
                        transNode.Translation = new Vector3(x, y, z * factor);
                        transNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
                            (float)(zRot * Math.PI / 180)) * Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.PiOver2);
                        transNode.Scale = Vector3.One * scale;

                        BuildingTransform editableBuildingTransformNode = new BuildingTransform(building, Settings.EditableScale);
                        //editableBuildingTransformNode.Translation = new Vector3(x, y, z * factor);
                        editableBuildingTransformNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
                            (float)(zRot * Math.PI / 180)) * Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.PiOver2);
                        editableBuildingTransformNode.Scale = Vector3.One * scale * Settings.EditableScale;

                        BuildingTransform realBuildingTransformNode = new BuildingTransform(building, 1);
                        realBuildingTransformNode.Translation = Vector3.UnitZ * -50;
                        /*
                        realBuildingTransformNode.Translation = new Vector3(x, y, z * factor);
                        realBuildingTransformNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ,
                            (float)(zRot * Math.PI / 180)) * Quaternion.CreateFromAxisAngle(Vector3.UnitX,
                            MathHelper.PiOver2);
                        realBuildingTransformNode.Scale = Vector3.One * scale;// *new Vector3(Settings.RealScale);
                        */
                        realBuildingTransformNode.real = true;

                        AirRightsTransform airRightsTransformNode = new AirRightsTransform(building);
                        airRightsTransformNode.Translation = new Vector3(graphNodeXOffset, graphNodeYOffset, 0);
                        
                        editableBuildingTransformNode.addObserver(transNode);
                        editableBuildingTransformNode.addObserver(realBuildingTransformNode);
                        editableBuildingTransformNode.addObserver(airRightsTransformNode);
                        editableBuildingTransformNode.addObserver(realBuildingTransformNode);

                        Material buildingMaterial = new Material();
                        buildingMaterial.Diffuse = Color.White.ToVector4();
                        buildingMaterial.Specular = Color.White.ToVector4();
                        buildingMaterial.SpecularPower = 3f;

                        building.Material = buildingMaterial;

                        Material editableBuildingMaterial = new Material();
                        editableBuildingMaterial.Diffuse = new Vector4(153,153,153,.7f);
                        editableBuildingMaterial.Specular = Color.White.ToVector4();
                        editableBuildingMaterial.SpecularPower = 3f;

                        editableBuilding.Material = editableBuildingMaterial;

                        Material realBuildingMaterial = new Material();
                        realBuildingMaterial.Diffuse = new Vector4(153, 153, 153, 1);
                        realBuildingMaterial.Specular = Color.White.ToVector4();
                        realBuildingMaterial.SpecularPower = 3f;

                        realBuilding.Material = editableBuildingMaterial;

                        building.TransformNode = transNode;
                        building.EditBuildingTransform = editableBuildingTransformNode;
                        editableBuilding.TransformNode = editableBuildingTransformNode;
                        building.AirRightsTransformNode = airRightsTransformNode;
                        building.RealBuildingTransform = realBuildingTransformNode;

                        ///////////// ADD CHILDREN TO PARENT NODES, WHICH WERE ALREADY ADDED TO GROUND

                        parentTrans.AddChild(transNode);
                        //parentTrans.AddChild(editableBuildingTransformNode);
                        lot.transformNode = transNode;

                        //parentTransBigOne.AddChild(realBuildingTransformNode);
                        //realBuildingTransformNode.AddChild(realBuilding);

                        transNode.AddChild(building);

                        editableBuildingTransformNode.ModelBuilding = building;
                        editableBuildingTransformNode.AddChild(editableBuilding);

                        realBuildingTransformNode.AddChild(realBuilding);

                        airRightsGraph.AddChild(airRightsTransformNode);
                        //Log.Write(airRightsNode.Name + " airRightsNode was loaded.");
                        airRightsTransformNode.AddChild(airRightsNode);
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            //UpdateMouse();
            if (AppStateMgr.continousMode
                && AppStateMgr.inState(AppState.Browse)
                && !menu.Visible)
            {
                if (selectedBuilding != null)
                    if (selectedBuilding.EditBuildingTransform != null)
                        selectedBuilding.EditBuildingTransform.Enabled = true;
                dataRepresentation.Enabled = false;
                getClosestBuilding(null);
            }

            //UpdateMouse();
            if (AppStateMgr.continousMode
                && AppStateMgr.inState(AppState.Edit)
                && !menu.Visible
                && !AppStateMgr.handleGrabbed)
            {
                if (selectedBuilding != null)
                    if (selectedBuilding.EditBuildingTransform != null)
                        selectedBuilding.EditBuildingTransform.Enabled = true;
                dataRepresentation.Enabled = false;
                getClosestHandle(null);
            }

            //UpdateMouse();
            if (AppStateMgr.continousMode
                && AppStateMgr.inState(AppState.Info)) //&& !menu.Visible
            {
                if (selectedBuilding != null)
                {
                    if (selectedBuilding != null)
                        if (selectedBuilding.EditBuildingTransform != null)
                            selectedBuilding.EditBuildingTransform.Enabled = false;
                    dataRepresentation.Enabled = true;
                    dataRepresentation.showData(selectedBuilding);
                }
            }
            
            if (AppStateMgr.handleGrabbed && !menu.Visible)
                ModificationManager.processWandMovement();

            cumulativeTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            //GoblinXNA.UI.Notifier.AddMessage("currentTime=" + cumulativeTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            if (AppStateMgr.showIcon)
            {
                GoblinXNA.UI.UI2D.UI2DRenderer.FillRectangle(
                    AppStateMgr.iconPlaceHolder,
                    AppStateMgr.icon(),
                    Color.White);
            }
        }

        protected void MouseCenter()
        {
            Mouse.SetPosition(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
        }

        protected void UpdateMouse(Vector2 selectionVector)
        {
            //GoblinXNA.UI.Notifier.AddMessage("Mouse Update!");

            if ((cumulativeTime - lastMenuSelectionTime) > Settings.MinTimeBetweenMenuSelections)
            {
                lastMenuSelectionTime = cumulativeTime;
                menu.HandleInput(selectionVector);
            }
            MouseCenter();

            //Console.WriteLine("x=" + Mouse.GetState().X + " y=" + Mouse.GetState().Y);
            //Console.WriteLine("x=" + Window.ClientBounds.Width + " y=" + Window.ClientBounds.Height);
        }

        private void MouseMoveHandler(Point mouseLocation)
        {
            int xDel = mouseLocation.X - centerX;
            int yDel = mouseLocation.Y - centerY;

            if (new Vector2((float)xDel, (float)yDel).Length() > Settings.MinGestureDistance) {
                UpdateMouse(new Vector2(xDel, yDel));
            }
        }

        private void KeyTypeHandler(Microsoft.Xna.Framework.Input.Keys key, KeyModifier modifier)
        {
            if (key == Keys.R)
            {
                flag = !flag;
            } 
            
            if (key == Microsoft.Xna.Framework.Input.Keys.Q)
            {
                this.Exit();
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.S)
            {
                Utilities.SaveScreenShot(graphics.GraphicsDevice, "ScreenShot.jpg");
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Space)
            {
                //MousePressHandler(MouseInput.LeftButton, new Point(centerX, centerY));
                if (AppStateMgr.inState(AppState.Browse))
                    getClosestBuilding(null);
                else if (AppStateMgr.inState(AppState.Edit))
                    getClosestHandle(null);
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Escape)
            {
                MousePressHandler(MouseInput.RightButton, new Point(centerX, centerY));
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.C)
            {
                AppStateMgr.continousMode = !AppStateMgr.continousMode;
                GoblinXNA.UI.Notifier.AddMessage("continousMode=" + AppStateMgr.continousMode);
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Up)
            {
                if ( (buildingSelected()) && (selectedEditableBuilding != null) )
                {
                    ModificationManager.addFloor(1);
                }
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Down)
            {
                if ((buildingSelected()) && (selectedEditableBuilding != null))
                {
                    ModificationManager.addFloor(-1);
                }
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Tab)
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

            if (key == Microsoft.Xna.Framework.Input.Keys.X)
            {

                float scale = 10.0f;

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
                {
                    scale = 1000f;
                }

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftShift)) {
                    bigOneTranslations.Translation = bigOneTranslations.Translation - (Vector3.UnitX * scale);
                } else {
                    bigOneTranslations.Translation = bigOneTranslations.Translation + (Vector3.UnitX * scale);
                }
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Y)
            {

                float scale = 10.0f;

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
                {
                    scale = 1000f;
                }

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    bigOneTranslations.Translation = bigOneTranslations.Translation - (Vector3.UnitY * scale);
                }
                else
                {
                    bigOneTranslations.Translation = bigOneTranslations.Translation + (Vector3.UnitY * scale);
                }
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.Z)
            {

                float scale = 10.0f;

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
                {
                    scale = 1000f;
                }

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    bigOneTranslations.Translation = bigOneTranslations.Translation - (Vector3.UnitZ * scale);
                }
                else
                {
                    bigOneTranslations.Translation = bigOneTranslations.Translation + (Vector3.UnitZ * scale);
                }
            }
            
            if (key == Microsoft.Xna.Framework.Input.Keys.B)
            {
                float scale = 10.0f;

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
                {
                    scale = 1000f;
                }

                if (Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    bigOneTranslations.Scale = bigOneTranslations.Scale - (new Vector3(1, 1, 1) * scale);
                }
                else
                {
                    bigOneTranslations.Scale = bigOneTranslations.Scale + (new Vector3(1, 1, 1) * scale);
                }
            }

            if (key == Microsoft.Xna.Framework.Input.Keys.L)
            {
                Log.Write("Scale=" + bigOneTranslations.Scale.ToString()
                    + " Translation=" + bigOneTranslations.Translation.ToString());

                Log.Write("BoundingFrustum" + this.scene.CameraNode.BoundingFrustum.Matrix.ToString());
            }

        }

        //private void loadData()
        //{
        //    dataRepresentation = new DataRepresentation(graphics.GraphicsDevice, font, color);
        //    groundMarkerNode.AddChild(dataRepresentation);
        //}

        private void MousePressHandler(int button, Point mouseLocation)
        {
            if (button == MouseInput.LeftButton)
            {
                //GoblinXNA.UI.Notifier.AddMessage("Left");
                if (!menu.Visible)
                {
                    //whooshup.Play();
                    menu.Show(AppStateMgr.currentMenu(), new Vector2((float)centerX, (float)centerY));
                }
            }
            else if (button == MouseInput.RightButton)
            {
                //GoblinXNA.UI.Notifier.AddMessage("Right");
                //whooshdown.Play();
                menu.Back();
            }
        }

        public void getClosestBuilding(Object sender)
        {
            if (!tool.Marker.MarkerFound) {
                if (!AppStateMgr.continousMode) GoblinXNA.UI.Notifier.AddMessage("Tool not found!");
                return;
            }

            //GoblinXNA.UI.Notifier.AddMessage("tool=" + tool.Marker.WorldTransformation.Translation.ToString());
            //GoblinXNA.UI.Notifier.AddMessage("ground=" + groundMarkerNode.WorldTransformation.Translation.ToString());

            float minDis = float.MaxValue;
            Building closestBuilding = null;

            int i = 0;
            foreach (Building b in buildings)
            {
                float dis = (b.getBaseWorld() - tool.getTranslationFromOrigin()).Length();

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

            if (closestBuilding != null && minDis < Settings.MinDistanceForGrab)
            {
                selectBuilding(closestBuilding);
            }

        }

        public void getClosestHandle(Object sender)
        {
            if (!tool.Marker.MarkerFound)
            {
                if (!AppStateMgr.continousMode) GoblinXNA.UI.Notifier.AddMessage("Tool not found!");
                return;
            }

            //GoblinXNA.UI.Notifier.AddMessage("tool=" + tool.Marker.WorldTransformation.Translation.ToString());
            //GoblinXNA.UI.Notifier.AddMessage("ground=" + groundMarkerNode.WorldTransformation.Translation.ToString());

            float minDis = float.MaxValue;
            Handle closestHandle = null;

            int i = 0;
            foreach (Handle h in handles)
            {
                Matrix t = h.GeoNode.WorldTransformation * h.GeoNode.MarkerTransform;
                float dis = (t.Translation - tool.Marker.WorldTransformation.Translation).Length();

                if (dis < minDis)
                {
                    minDis = dis;
                    closestHandle = h;
                    closestIndex = i;
                }

                i++;

                /*
                Log.Write(h.Name
                    + ", h.GeoNode.WorldTransformation=" + h.GeoNode.WorldTransformation.Translation.ToString()
                    + ", h.GeoNode.MarkerTransform=" + h.GeoNode.MarkerTransform.Translation.ToString()
                    + ", t.Translation=" + t.Translation.ToString()
                    + ", marker.WorldTransformation=" + tool.Marker.WorldTransformation.Translation.ToString()
                    + ", dis=" + dis);
                */          

            }

            //Log.Write("closestHandle.Name=" + closestHandle.Name);

            if (closestHandle != null && minDis < Settings.MinDistanceForGrab)
            {
                selectHandle(closestHandle);
            }

        }

        private void selectHandle(Handle h)
        {
            if (selectedHandle == h)
                return;
            
            hum.Play();

            if (selectedHandle != null)
            {
                selectedHandle.GeoNode.Material.Diffuse = Color.DarkBlue.ToVector4();
            }

            selectedHandle = h;

            selectedHandle.GeoNode.Material.Diffuse = Color.Green.ToVector4();

            if (!AppStateMgr.continousMode) GoblinXNA.UI.Notifier.AddMessage(selectedHandle.Name);

        }

        private void selectBuilding(Building b)
        {

            if (selectedBuilding != b)
            {

                votesForChangingSelectedBuilding++;

                if (votesForChangingSelectedBuilding < Settings.VotesForChangingSelectedBuilding)
                {
                    return;
                }
                else
                {
                    votesForChangingSelectedBuilding = 0;
                }

                hum.Play();
            }
            else // (selectedBuilding == b)
            {
                return;
            }
            
            if (buildingSelected())
            {
                selectedBuilding.Material.Diffuse = Color.White.ToVector4();
                //if (selectedEditableBuilding != null)
                //{
                parentTransEditable.RemoveChild(selectedBuilding.EditBuildingTransform);
                //}
                //selectedBuilding.setEditableTransform(null);
            }

            selectedBuilding = b;
            selectedEditableBuilding = editableBuildings[selectedBuilding];
            selectedLot = b.Lot;

            selectedBuilding.Material.Diffuse = Color.DarkSlateGray.ToVector4();
            parentTransEditable.AddChild(selectedBuilding.EditBuildingTransform);

            handles[(int)Handle.Location.Top].Translation = b.CenterOfCeilWithOffset;
            handles[(int)Handle.Location.BottomSW].Translation = b.MinPointWithOffset;
            handles[(int)Handle.Location.BottomSE].Translation = new Vector3(b.MaxPointWithOffset.X, b.MinPointWithOffset.Y, b.MinPointWithOffset.Z);
            handles[(int)Handle.Location.BottomNE].Translation = new Vector3(b.MaxPointWithOffset.X, b.MaxPointWithOffset.Y, b.MinPointWithOffset.Z);
            handles[(int)Handle.Location.BottomNW].Translation = new Vector3(b.MinPointWithOffset.X, b.MaxPointWithOffset.Y, b.MinPointWithOffset.Z);

            if (!AppStateMgr.continousMode) GoblinXNA.UI.Notifier.AddMessage(selectedBuilding.Name);

            //dataRepresentation.Enabled = true;
            //dataRepresentation.showData(b);
        }

        private void addToFootprint(BuildingTransform eb)
        {
            //if ( (thisXscale < MaxXScale) || (thisZScale < MaxZScale) ) 
            //  eb.Scale = movement your mouse has made
        }


        public void initializeHandles()
        {
            
            foreach (Handle.Location locationItem in Enum.GetValues(typeof(Handle.Location)))
            {
                Material handleMaterial = new Material();
                handleMaterial.Specular = Color.White.ToVector4();
                handleMaterial.Diffuse = Color.DarkBlue.ToVector4();
                handleMaterial.SpecularPower = 10;

                Handle h = new Handle(locationItem, "Handle" + (int)locationItem, handleMaterial);
                handles.Add(h);
                handleTrans.AddChild(h);
                h.Enabled = (AppStateMgr.handlesEnabled && (h.Loc == Handle.Location.Top));
            }
            
            foreach (Building b in buildings)
            {
                foreach (Handle h in handles)
                {
                    b.EditBuildingTransform.addObserver(h);
                }
                
                /*
                Material handleMaterial = new Material();
                handleMaterial.Specular = Color.White.ToVector4();
                handleMaterial.Diffuse = Color.DarkBlue.ToVector4();
                handleMaterial.SpecularPower = 10;

                Handle h = new Handle(Handle.Location.Top, "Handle" + b.Name, handleMaterial);

                h.Translation = b.CenterOfCeilWithoutOffset;// +new Vector3(-3.326081f, 19.7829f, 0f);
                groundMarkerNode.AddChild(h);
                GoblinXNA.UI.Notifier.AddMessage(b.CenterOfBaseWithoutOffset.ToString());
                 */
            }
            
        }

        public void toggleHandles()
        {
            foreach (Handle h in handles)
            {
                h.Enabled = (AppStateMgr.handlesEnabled && (h.Loc == Handle.Location.Top));
            }
            
        }

        public bool buildingSelected()
        {
            return (selectedBuilding != null);
        }

        internal Vector3 getWandLocation()
        {
            return toolMarkerNode.WorldTransformation.Translation;
        }
    }
}
