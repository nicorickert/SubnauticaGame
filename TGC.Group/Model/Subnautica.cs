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
        private TgcScene scene;
        
        public Player Player { get; private set; }
        public List<HeightMapTextured> heightMaps = new List<HeightMapTextured>();


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

            // Genero el terreno
            heightMaps.Add(new HeightMapTextured(this, "SeaFloor", new TGCVector3(0, -3000, 0), MediaDir + "Terrain\\" + "HMInclinado.jpg", MediaDir + "Terrain\\" + "image.png"));

            heightMaps.Add(new HeightMapTextured(this, "Mar", new TGCVector3(0, 10, 0), MediaDir + "Terrain\\" + "HeightMapPlano.jpg", MediaDir + "Terrain\\" + "blueTransparent.png"));

            foreach (HeightMapTextured hm in heightMaps)
            {
                hm.Init();
            }

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

            scene.RenderAll();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Render();

            PostRender();
        }

        public override void Dispose()
        {
            foreach (GameObject o in sceneObjects)
                o.Dispose();

            // HeightMaps
            foreach (HeightMapTextured hm in heightMaps)
                hm.Dispose();

            scene.DisposeAll();
        }

        #endregion

        #region INTERFACE

        public void InstanceObject(GameObject obj)
        {
            sceneObjects.Add(obj);
        }

        #endregion

        private void LoadMainScene()
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            scene = loader.loadSceneFromFile(MediaDir + "Scene\\Isla-TgcScene.xml");

            /* OBJETOS INDIVIDUALES */
            InstanceObject(new StaticObject(this, "coral", new TGCVector3(0,0, 0), TGCVector3.One * 3, new TGCVector3(0,0,0), MediaDir + "Aquatic\\Meshes\\coral-TgcScene.xml"));
        }
    }
}
