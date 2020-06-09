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
using TGC.Group.Model.Items;
using TGC.Core.BoundingVolumes;

namespace TGC.Group.Model
{
    public class Subnautica : TGCExample
    {
        #region MESHES
        private List<TgcMesh> playerMeshes;
        private List<TgcMesh> shipMeshes;
        #endregion

        #region SETTINGS

        private TGCVector3 skyBoxDimensions = new TGCVector3(65000, 20000, 65000);
        public SueloDelMar SueloDelMar { get; private set; }
        public List<HeightMapTextured> heightMaps = new List<HeightMapTextured>();
        private TgcSkyBox skyBox;
        private List<GameObject> removedObjects = new List<GameObject>();
        private float time = 0f;
        private readonly float waterY = 0f;
        private readonly float floorY = -6500;
        private float escapeDelay = 0;

        public bool MouseEnabled { get; private set; } = false;
        public bool FocusInGame { get; private set; } = true; // Variable para saber si estoy jugando o en menu

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
            SetCamera();
        }

        public override void Update()
        {
            PreUpdate();
            escapeDelay += ElapsedTime;

            UpdateHUD();

            if (Input.keyDown(Key.Escape) && escapeDelay > 0.5f) { // uso el delay porque no me funciona el keyUp o keyPressed
                escapeDelay = 0;
                FocusInGame = !FocusInGame;
                ManageFocus();
            }

            if (FocusInGame)    // Si no se está en modo gameplay, desactivar el update de todo
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

            time += ElapsedTime;
            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            RenderHUD();
            skyBox.Render();

            foreach (GameObject o in SceneObjects)
                o.Render();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Render();


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

            ItemDatabase.Instance.Dispose();
            skyBox.Dispose();
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

        // heightmaps: 0 = piso, 1 = agua
        public float WaterLevelToWorldHeight(float waterLevel) => heightMaps[1].CalcularAltura(Player.Position.X, Player.Position.Z) + waterLevel;

        public float FloorLevelToWorldHeight(float floorLevel) => heightMaps[0].CalcularAltura(Player.Position.X, Player.Position.Z) + floorLevel;

        public void MouseDisable()
        {
            MouseEnabled = false;
            int deviceWidth = D3DDevice.Instance.Width;
            int deviceHeight = D3DDevice.Instance.Height;
            Cursor.Clip = new Rectangle(deviceWidth / 2, deviceHeight / 2, 1, 1); // El cursor se queda quieto en un punto y permite que se pueda mover la camara infinitamente
            Cursor.Hide();
        }

        public void MouseEnable()
        {
            MouseEnabled = true;
            Cursor.Clip = new Rectangle(); // libero el mouse
            Cursor.Show();
        }

        #endregion

        #region PRIVATE_METHODS

        private void ManageFocus()
        {
            if (FocusInGame)
            {
                MouseDisable();
            }
            else
            {
                if (MouseEnabled)
                {
                    Player.CloseInventory();
                    Ship.CloseCraftingMenu();
                }
                MouseEnable();
            }
        }

        private void SetCamera()
        {
            // Cambio el farPlane
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 10f).ToMatrix();

            Camera = new FPSCamera(this, Player, Player.RelativeEyePosition);
        }

        private void LoadMainScene()
        {
            // Genero el terreno

            LoadSkybox();

            Player = new Player(this, "player", playerMeshes);
            InstanceObject(Player);

            Ship = new Ship(this, "main_ship", shipMeshes);
            InstanceObject(Ship);

            LoadTerrain();
        }

        private void LoadTerrain()
        {
            SueloDelMar = new SueloDelMar(this, "SeaFloor", new TGCVector3(0, floorY, 0), MediaDir + "Terrain\\" + "HMFondo-x128.jpg", MediaDir + "Terrain\\" + "sand.jpg", ShadersDir + "SeaFloorShader.fx", 500f, 20f);
            heightMaps.Add(SueloDelMar);
            heightMaps.Add(new HeightMapTextured(this, "Mar", new TGCVector3(0, waterY, 0), MediaDir + "Terrain\\" + "HeightMapPlano.jpg", MediaDir + "Skybox\\down.jpg", ShadersDir + "WaterShader.fx", 1000f, 1f));

            foreach (HeightMapTextured hm in heightMaps)
            {
                hm.Init();
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
        }

        private void RenderHUD()
        {
            healthTextBox.render();
            oxygenTextBox.render();
            screenCenter.render();
            gameTimer.render();
        }

        private void DisposeHUD()
        {
            healthTextBox.Dispose();
            oxygenTextBox.Dispose();
            gameTimer.Dispose();
        }

        private void InitMainMeshes()
        {
            var loader = new TgcSceneLoader();

            playerMeshes = loader.loadSceneFromFile(MediaDir + "Player\\Player-TgcScene.xml").Meshes;
            shipMeshes = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\ship-TgcScene.xml").Meshes;
        }

        private void LoadSkybox()
        {
            skyBox = new TgcSkyBox();
            skyBox.Center = TGCVector3.Up * (skyBoxDimensions.Y / 11f);
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
