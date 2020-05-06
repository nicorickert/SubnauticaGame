using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    class StaticObject : GameObject
    {
        public StaticObject(Subnautica gameInstance, string name, TgcMesh mesh, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation) : base(gameInstance, name, mesh)
        {
            Mesh.Position = position;
            Mesh.Rotation = rotation;
            Mesh.Scale = scale;

            TGCMatrix scaleTransform = TGCMatrix.Scaling(Mesh.Scale);
            TGCMatrix translation = TGCMatrix.Translation(Mesh.Position);
            TGCMatrix rotationTransform = TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z);

            Mesh.Transform = scaleTransform * rotationTransform * translation;
        }

        public StaticObject(Subnautica gameInstance, string name, TgcMesh mesh, TGCVector3 position, float scale, TGCVector3 rotation)
            : this(gameInstance, name, mesh, position, TGCVector3.One * scale, rotation) { }

        public StaticObject(Subnautica gameInstance, string name, TgcMesh mesh, TGCVector3 position, float scale)
            : this(gameInstance, name, mesh, position, TGCVector3.One * scale, TGCVector3.Empty) { }

        public StaticObject(Subnautica gameInstance, string name, TgcMesh mesh, TGCVector3 position, TGCVector3 rotation)
            : this(gameInstance, name, mesh, position, TGCVector3.One, rotation) { }

        public StaticObject(Subnautica gameInstance, string name, TgcMesh mesh, TGCVector3 position)
            : this(gameInstance, name, mesh, position, TGCVector3.One, TGCVector3.Empty) { }

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
