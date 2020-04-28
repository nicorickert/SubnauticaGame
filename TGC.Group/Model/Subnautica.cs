using System.Collections.Generic;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    public class Subnautica : TGCExample
    {
        private List<GameObject> sceneObjects = new List<GameObject>();
        private TgcScene island;
        private List<HeightMapTextured> heightMaps = new List<HeightMapTextured>();

        public Player Player { get; private set; }
        public TgcScene Ship { get; private set; }
        public float FloorY { get; } = -3000;
        public float WaterY { get; } = 0;


        public Subnautica(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        #region TGC_EXAMPLE

        public override void Init()
        {
            Player = new Player(this, "player");
            InstanceObject(Player); //Tal vez no sea necesario meter al Player dentro de la bolsa de GameObjects

            Camera = new FPSCamera(Player, new TGCVector3(0, 120, -20));

            LoadMainScene();
        }

        public override void Update()
        {
            PreUpdate();

            // Objetos
            foreach (GameObject o in sceneObjects)
                o.Update();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Update();

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

            island.RenderAll();
            Ship.RenderAll();

            PostRender();
        }

        public override void Dispose()
        {
            foreach (GameObject o in sceneObjects)
                o.Dispose();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Dispose();

            island.DisposeAll();
            Ship.DisposeAll();
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
            LoadTerrain();

            // Isla
            TgcSceneLoader loader = new TgcSceneLoader();
            island = loader.loadSceneFromFile(MediaDir + "Scene\\Isla-TgcScene.xml");

            /* Cargo el barco, probablemente sea una clase individual en el futuro como "Player" */
            LoadShip();

            InstanceObject(new Fish(this, "pecesito", TGCVector3.Empty));

            /* OBJETOS INDIVIDUALES */
            InstanceObject(new StaticObject(this, "coral", new TGCVector3(500, FloorY + 500, 0), 5, MediaDir + "Aquatic\\Meshes\\coral-TgcScene.xml"));
        }

        private void LoadShip()
        {
            TgcSceneLoader loader = new TgcSceneLoader();

            Ship = loader.loadSceneFromFile(MediaDir + "Aquatic\\Meshes\\ship-TgcScene.xml");
            foreach (TgcMesh mesh in Ship.Meshes)
            {
                mesh.Position += new TGCVector3(3500, 60, 0);   // seteo la posicion del barco
                mesh.Scale *= 3;
                mesh.Rotation += new TGCVector3(0, FastMath.PI_HALF, 0);

                TGCMatrix translation = TGCMatrix.Translation(mesh.Position);
                TGCMatrix scaling = TGCMatrix.Scaling(mesh.Scale);
                TGCMatrix rotation = TGCMatrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z);

                mesh.Transform = rotation * scaling * translation;
            }
        }

        private void LoadTerrain()
        {
            heightMaps.Add(new HeightMapTextured(this, "SeaFloor", new TGCVector3(0, FloorY, 0), MediaDir + "Terrain\\" + "HMInclinado.jpg", MediaDir + "Terrain\\" + "image.png"));
            heightMaps.Add(new HeightMapTextured(this, "Mar", new TGCVector3(0, WaterY, 0), MediaDir + "Terrain\\" + "HeightMapPlano.jpg", MediaDir + "Terrain\\" + "blueTransparent.png"));

            foreach (HeightMapTextured hm in heightMaps)
            {
                hm.Init();
            }
        }

        #endregion
    }
}
