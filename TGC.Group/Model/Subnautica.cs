using System.Collections.Generic;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    class Subnautica : TGCExample
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
            // Inicializar recursos
            var loader = new TgcSceneLoader();

            Player = new Player(this, "player");
            InstanceObject(Player);

            Camera = new FPSCamera(Player, new TGCVector3(0, 100, 200));

            scene = loader.loadSceneFromFile(MediaDir + "Scene\\Isla-TgcScene.xml");
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
    }
}
