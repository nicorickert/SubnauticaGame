using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class StaticObject : GameObject
    {
        public StaticObject(Subnautica gameInstance, string name, TGCVector3 position, string meshPath) : base(gameInstance, name)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Mesh = loader.loadSceneFromFile(meshPath).Meshes[0];

            Mesh.Position = position;
            Mesh.Transform = TGCMatrix.Translation(Mesh.Position);
        }

        public override void Update() { }

        public override void Render()
        {
            Mesh.Render();
        }

        public override void Dispose()
        {
            Mesh.Dispose();
        }
    }
}
