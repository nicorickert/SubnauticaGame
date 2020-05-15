using System;
using System.Collections.Generic;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model.Utils;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Terrain;
using TGC.Core.Text;
using System.Drawing;
using TGC.Core.Collision;
using TGC.Core.Input;

namespace TGC.Group.Model
{
    public class Subnautica : TGCExample
    {
        #region MESHES
        private List<TgcMesh> playerMeshes;
        private TgcMesh coralMesh;
        private List<TgcMesh> shipMeshes;
        #endregion

        #region SETTINGS

        private TGCVector3 skyBoxDimensions = new TGCVector3(40000, 10000, 40000);
        private TgcScene island;
        private List<HeightMapTextured> heightMaps = new List<HeightMapTextured>();
        private TgcSkyBox skyBox;
        private List<GameObject> removedObjects = new List<GameObject>();
        private float time = 0f;

        public float FloorY { get; } = -3000;
        public float WaterY { get; } = 0;
        public float escapeDelay = 0;
        public bool focusInGame = true; // Variable para saber si estoy jugando o en menu

        #endregion

        #region OBJECTS
        public List<GameObject> SceneObjects { get; private set; } = new List<GameObject>();
        public Player Player { get; private set; }
        public Ship Ship { get; private set; }

        private SpawnManager spawnManager;
        #endregion

        #region HUD

        private TgcText2D healthTextBox = new TgcText2D();
        private TgcText2D oxygenTextBox = new TgcText2D();
        private TgcText2D screenCenter = new TgcText2D();
        private TgcText2D inventoryHud = new TgcText2D();
        private TgcText2D selectedItem = new TgcText2D();
        private TgcText2D gameTimer = new TgcText2D();

        #endregion


        public Subnautica(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }


        #region TGC_EXAMPLE

        public override void Init()
        {
            InitMainMeshes();
            InitHUD();
            LoadMainScene();
            ManageFocus();
            spawnManager = new SpawnManager(this);

            // Cambio el farPlane
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 3f).ToMatrix();

            Camera = new FPSCamera(Player, new TGCVector3(0, 120, 30));
        }

        public override void Update()
        {
            PreUpdate();
            escapeDelay += ElapsedTime;

            if (Input.keyDown(Key.Escape) && escapeDelay > 0.5f) { // uso el delay porque no me funciona el keyUp o keyPressed
                escapeDelay = 0;
                focusInGame = !focusInGame;
                ManageFocus();
            }

            if (focusInGame)    // Si no se está en modo gameplay, desactivar el update de todo
            {
                UpdateInstantiatedObjects();
                spawnManager.Update();

                // Objetos
                foreach (GameObject o in SceneObjects)
                    o.Update();

                // HeightMaps
                foreach (HeightMapTextured hm in heightMaps)
                    hm.Update();

                // Muevo el centro del skybox para que sea inalcanzable
                skyBox.Center = new TGCVector3(Camera.Position.X, 0, Camera.Position.Z);
            }

            UpdateHUD();

            time += ElapsedTime;
            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            RenderHUD();

            foreach (GameObject o in SceneObjects)
                o.Render();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Render();

            skyBox.Render();
            island.RenderAll();
            island.Meshes.ForEach(mesh => mesh.BoundingBox.Render());

            PostRender();
        }

        public override void Dispose()
        {
            DisposeHUD();
            spawnManager.Dispose();

            foreach (GameObject o in SceneObjects)
                o.Dispose();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Dispose();

            skyBox.Dispose();
            island.DisposeAll();
        }

        #endregion

        #region INTERFACE

        public void InstanceObject(GameObject obj)
        {
            SceneObjects.Add(obj);
        }

        public void DestroyObject(GameObject obj)
        {
            removedObjects.Add(obj);
        }

        #endregion

        #region PRIVATE_METHODS

        private void LoadMainScene()
        {
            // Genero el terreno

            LoadSkybox();

            Player = new Player(this, "player", playerMeshes);
            InstanceObject(Player); //Tal vez no sea necesario meter al Player dentro de la bolsa de GameObjects

            Ship = new Ship(this, "main_ship", shipMeshes);
            InstanceObject(Ship);

            LoadTerrain();

            TgcSceneLoader loader = new TgcSceneLoader();
            island = loader.loadSceneFromFile(MediaDir + "Scene\\Isla-TgcScene.xml");

            /* OBJETOS INDIVIDUALES */
            InstanceObject(new StaticObject(this, "coral1", new List<TgcMesh>(new TgcMesh[] { coralMesh.createMeshInstance("coral1") }), new TGCVector3(500, FloorY + 500, 0), 5));
            InstanceObject(new StaticObject(this, "coral2", new List<TgcMesh>(new TgcMesh[] { coralMesh.createMeshInstance("coral1") }), new TGCVector3(1000, FloorY + 500, 300), 5));

            InstanceObject(new StaticObject(this, "coral3", new List<TgcMesh>(new TgcMesh[] { coralMesh.createMeshInstance("coral1") }), new TGCVector3(3000, FloorY + 300, -1000), 5));
            InstanceObject(new StaticObject(this, "coral4", new List<TgcMesh>(new TgcMesh[] { coralMesh.createMeshInstance("coral1") }), new TGCVector3(3500, FloorY + 300, -700), 5));

            InstanceObject(new StaticObject(this, "coral5", new List<TgcMesh>(new TgcMesh[] { coralMesh.createMeshInstance("coral1") }), new TGCVector3(300, FloorY + 700, 2800), 5));

            InstanceObject(new StaticObject(this, "coral6", new List<TgcMesh>(new TgcMesh[] { coralMesh.createMeshInstance("coral1") }), new TGCVector3(1000, FloorY + 300, -3000), 5));
        }

        private void LoadTerrain()
        {
            heightMaps.Add(new HeightMapTextured(this, "SeaFloor", new TGCVector3(0, FloorY, 0), MediaDir + "Terrain\\" + "HMInclinado.jpg", MediaDir + "Terrain\\" + "sand.jpg", ShadersDir + "SeaFloorShader.fx"));
            heightMaps.Add(new HeightMapTextured(this, "Mar", new TGCVector3(0, WaterY, 0), MediaDir + "Terrain\\" + "HeightMapPlano.jpg", MediaDir + "Skybox\\down.jpg", ShadersDir + "WaterShader.fx"));

            foreach (HeightMapTextured hm in heightMaps)
            {
                hm.Init();
            }
        }

        private void ManageFocus()
        {
            if (focusInGame)
            {
                int deviceWidth = D3DDevice.Instance.Width;
                int deviceHeight = D3DDevice.Instance.Height;
                Cursor.Clip = new Rectangle(deviceWidth/2, deviceHeight/2, 1, 1); // El cursor se queda quieto en un punto y permite que se pueda mover la camara infinitamente
                Cursor.Hide();
            }
            else
            {
                Cursor.Clip = new Rectangle(); // libero el mouse
                Cursor.Show();
            }
        }

        private void InitHUD()
        {
            int deviceWidth = D3DDevice.Instance.Width;
            int deviceHeight = D3DDevice.Instance.Height;

            healthTextBox.Text = "✚ 100";
            healthTextBox.Color = Color.DarkOrange;
            healthTextBox.Position = new Point((int)FastMath.Floor(0.05f * deviceWidth), (int)FastMath.Floor(0.9f * deviceHeight));
            healthTextBox.Size = new Size(600, 200);
            healthTextBox.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
            healthTextBox.Align = TgcText2D.TextAlign.LEFT;

            oxygenTextBox.Text = "◴ 100";
            oxygenTextBox.Color = Color.DarkOrange;
            oxygenTextBox.Position = new Point((int)FastMath.Floor(0.12f * deviceWidth), (int)FastMath.Floor(0.9f * deviceHeight));
            oxygenTextBox.Size = new Size(600, 200);
            oxygenTextBox.changeFont(new Font("TimesNewRoman", 25, FontStyle.Bold));
            oxygenTextBox.Align = TgcText2D.TextAlign.LEFT;

            screenCenter.Text = "+";
            screenCenter.Color = Color.White;
            screenCenter.Position = new Point(deviceWidth/2 - 8, deviceHeight/2 - 40);
            screenCenter.Size = new Size(600, 200);
            screenCenter.changeFont(new Font("TimesNewRoman", 25, FontStyle.Regular));
            screenCenter.Align = TgcText2D.TextAlign.LEFT;

            inventoryHud.Text = "Inventory: ";
            inventoryHud.Color = Color.DarkOrange;
            inventoryHud.Position = new Point((int)FastMath.Floor(0.8f * deviceWidth), (int)FastMath.Floor(0.4f * deviceHeight));
            inventoryHud.Size = new Size(300, 600);
            inventoryHud.changeFont(new Font("TimesNewRoman", 25, FontStyle.Regular));
            inventoryHud.Align = TgcText2D.TextAlign.LEFT;

            selectedItem.Text = "Selected item: ";
            selectedItem.Color = Color.DarkOrange;
            selectedItem.Position = new Point((int)FastMath.Floor(0.8f * deviceWidth), (int)FastMath.Floor(0.38f * deviceHeight));
            selectedItem.Size = new Size(300, 300);
            selectedItem.changeFont(new Font("TimesNewRoman", 14, FontStyle.Bold));
            selectedItem.Align = TgcText2D.TextAlign.LEFT;

            gameTimer.Text = "Time: ";
            gameTimer.Color = Color.DarkOrange;
            gameTimer.Position = new Point((int)FastMath.Floor(0.05f * deviceWidth), (int)FastMath.Floor(0.85f * deviceHeight));
            gameTimer.Size = new Size(300, 600);
            gameTimer.changeFont(new Font("TimesNewRoman", 12, FontStyle.Regular));
            gameTimer.Align = TgcText2D.TextAlign.LEFT;
        }

        private void UpdateHUD()
        {
            healthTextBox.Text = "✚ " + Player.Health;
            oxygenTextBox.Text = "◴ " + Player.Oxygen;
            gameTimer.Text = "Time: " + FastMath.Floor(time) + " sec";
            selectedItem.Text = "Selected item: " + (Player.SelectedItem + 1);

            int index = 1;
            inventoryHud.Text = "Inventory:\n\n";
            foreach (var item in Player.Inventory.Items)
            {
                inventoryHud.Text += index + ") " + item.Name + "\n";
                index++;
            }
        }

        private void RenderHUD()
        {
            healthTextBox.render();
            oxygenTextBox.render();
            screenCenter.render();
            inventoryHud.render();
            selectedItem.render();
            gameTimer.render();
        }

        private void DisposeHUD()
        {
            healthTextBox.Dispose();
            oxygenTextBox.Dispose();
            inventoryHud.Dispose();
            selectedItem.Dispose();
            gameTimer.Dispose();
        }

        private void InitMainMeshes()
        {
            var loader = new TgcSceneLoader();

            playerMeshes = loader.loadSceneFromFile(MediaDir + "Player\\Player-TgcScene.xml").Meshes;
            coralMesh = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\coral-TgcScene.xml").Meshes[0];  // Que hacemos con esto :S
            shipMeshes = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\ship-TgcScene.xml").Meshes;
        }

        private void LoadSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Up * (skyBoxDimensions.Y / 10);
            skyBox.Size = new TGCVector3(skyBoxDimensions);

            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, MediaDir + "Skybox\\up.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, MediaDir + "Skybox\\down.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, MediaDir + "Skybox\\left.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, MediaDir + "Skybox\\right.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, MediaDir + "Skybox\\front.jpg");
            skyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, MediaDir + "Skybox\\back.jpg");

            skyBox.Init();
        }

        private void UpdateInstantiatedObjects()
        {
            SceneObjects.RemoveAll(obj => removedObjects.Contains(obj));
            removedObjects.Clear();
        }

        #endregion
    }
}
