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

namespace TGC.Group.Model
{
    public class Subnautica : TGCExample
    {
        #region MESHES
        private TgcMesh playerMesh;
        private TgcMesh fishMesh;
        private TgcMesh coralMesh;
        private TgcScene shipScene;
        #endregion

        private TgcScene island;
        private List<HeightMapTextured> heightMaps = new List<HeightMapTextured>();
        private TgcSkyBox skyBox;
        private TGCVector3 skyBoxDimensions = new TGCVector3(40000, 10000, 40000);

        private List<GameObject> sceneObjects = new List<GameObject>();

        public Player Player { get; private set; }
        public Ship Ship { get; private set; }
        public float FloorY { get; } = -3000;
        public float WaterY { get; } = 0;
        public float escapeDelay = 0;

        public bool focusInGame = true; // Variable para saber si estoy jugando o en menu

       
        public Subnautica(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        #region TGC_EXAMPLE

        public override void Init()
        {
            InitBaseMeshes();
            LoadMainScene();
            UpdateHUD();

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
                UpdateHUD();
            }

            if (focusInGame)    // Si no se está en modo gameplay, desactivar el update de todo
            {
                // Objetos
                foreach (GameObject o in sceneObjects)
                    o.Update();

                // HeightMaps
                foreach (HeightMapTextured hm in heightMaps)
                    hm.Update();

                // Muevo el centro del skybox para que sea inalcanzable
                skyBox.Center = new TGCVector3(Camera.Position.X, 0, Camera.Position.Z);
            }

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            foreach (GameObject o in sceneObjects)
                o.Render();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Render();

            skyBox.Render();
            island.RenderAll();

            PostRender();
        }

        public override void Dispose()
        {
            foreach (GameObject o in sceneObjects)
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
            sceneObjects.Add(obj);
        }

        #endregion

        #region PRIVATE_METHODS

        private void LoadMainScene()
        {
            // Genero el terreno

            LoadSkybox();

            Player = new Player(this, "player", new List<TgcMesh>(new TgcMesh[] { playerMesh }));
            InstanceObject(Player); //Tal vez no sea necesario meter al Player dentro de la bolsa de GameObjects

            Ship = new Ship(this, "main_ship", shipScene.Meshes);
            InstanceObject(Ship);

            /* 20 peces */
            SpawnFishes();

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
            heightMaps.Add(new HeightMapTextured(this, "SeaFloor", new TGCVector3(0, FloorY, 0), MediaDir + "Terrain\\" + "HMInclinado.jpg", MediaDir + "Terrain\\" + "image.png", null));
            heightMaps.Add(new HeightMapTextured(this, "Mar", new TGCVector3(0, WaterY, 0), MediaDir + "Terrain\\" + "HeightMapPlano.jpg", MediaDir + "Skybox\\down.jpg", ShadersDir + "WaterShader.fx"));

            foreach (HeightMapTextured hm in heightMaps)
            {
                hm.Init();
            }
        }

        // Spawnea 20 peces
        private void SpawnFishes()
        {
            for(int i = 0; i < 20; i++)
            {
                TGCVector3 spawnLocation = RandomSpawnLocation();
                spawnLocation.Y = MathExtended.GetRandomNumberBetween((int)FloorY + 900, (int)WaterY - 100);
                string name = "fish" + i;
                InstanceObject(new Fish(this, name, new List<TgcMesh>(new TgcMesh[] { fishMesh.createMeshInstance(name) }), spawnLocation));
            }
        }

        private TGCVector3 RandomSpawnLocation()
        {
            Random random = new Random();
            TGCVector3 spawnLocation = TGCVector3.Empty;
            spawnLocation.X = MathExtended.GetRandomNumberBetween(-5000, 5000);
            spawnLocation.Y = MathExtended.GetRandomNumberBetween(-5000, 5000);
            spawnLocation.Z = MathExtended.GetRandomNumberBetween(-5000, 5000);

            return spawnLocation;
        }

        private void UpdateHUD()
        {
            if (focusInGame)
            {
                
                Cursor.Clip = new System.Drawing.Rectangle(Cursor.Position.X, Cursor.Position.Y, 1, 1); // El cursor se queda quieto en un punto y permite que se pueda mover la camara infinitamente
                Cursor.Hide();
            }
            else
            {
                Cursor.Clip = new System.Drawing.Rectangle(); // libero el mouse
                Cursor.Show();
            }
        }

        private void InitBaseMeshes()
        {
            var loader = new TgcSceneLoader();

            playerMesh = loader.loadSceneFromFile(MediaDir + "Player\\Player-TgcScene.xml").Meshes[0];
            fishMesh = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\fish-TgcScene.xml").Meshes[0];
            coralMesh = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\coral-TgcScene.xml").Meshes[0];
            shipScene = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\ship-TgcScene.xml");
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

        #endregion
    }
}
