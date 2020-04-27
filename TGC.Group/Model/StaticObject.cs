using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class StaticObject : GameObject
    {
        public StaticObject(Subnautica gameInstance, string name, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation, string meshPath) : base(gameInstance, name)
        {
            TgcSceneLoader loader = new TgcSceneLoader();
            Mesh = loader.loadSceneFromFile(meshPath).Meshes[0];

            Mesh.Position = position;
            Mesh.Rotation = rotation;
            Mesh.Scale = scale;

            TGCMatrix scaleTransform = TGCMatrix.Scaling(Mesh.Scale);
            TGCMatrix translation = TGCMatrix.Translation(Mesh.Position);
            TGCMatrix rotationTransform = TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z);

            Mesh.Transform = scaleTransform * rotationTransform * translation;
        }

        public override void Update() { /* No tienen logica */ }

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
