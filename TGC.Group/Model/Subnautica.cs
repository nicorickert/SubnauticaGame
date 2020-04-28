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

            foreach (GameObject o in sceneObjects)
                o.Update();

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            foreach (GameObject o in sceneObjects)
                o.Render();

            scene.RenderAll();

            PostRender();
        }

        public override void Dispose()
        {
            foreach (GameObject o in sceneObjects)
                o.Dispose();

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

            InstanceObject(new Fish(this, "pecesito", TGCVector3.Empty));

            /* OBJETOS INDIVIDUALES */
            //InstanceObject(new StaticObject(this, "coral", TGCVector3.Empty, TGCVector3.One * 2, TGCVector3.Empty, MediaDir + "Aquatic\\Meshes\\coral-TgcScene.xml"));
        }
    }
}
