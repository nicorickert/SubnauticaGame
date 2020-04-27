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

        public StaticObject(Subnautica gameInstance, string name, TGCVector3 position, float scale, TGCVector3 rotation, string meshPath)
            : this(gameInstance, name, position, TGCVector3.One * scale, rotation, meshPath) { }

        public StaticObject(Subnautica gameInstance, string name, TGCVector3 position, float scale, string meshPath)
            : this(gameInstance, name, position, TGCVector3.One * scale, TGCVector3.Empty, meshPath) { }

        public StaticObject(Subnautica gameInstance, string name, TGCVector3 position, TGCVector3 rotation, string meshPath)
            : this(gameInstance, name, position, TGCVector3.One, rotation, meshPath) { }

        public StaticObject(Subnautica gameInstance, string name, TGCVector3 position, string meshPath)
            : this(gameInstance, name, position, TGCVector3.One, TGCVector3.Empty, meshPath) { }

        #region GameObject

        public override void Update() { /* No tienen logica */ }

        public override void Render()
        {
            Mesh.Render();
        }

        public override void Dispose()
        {
            Mesh.Dispose();
        }

        #endregion
    }
}
