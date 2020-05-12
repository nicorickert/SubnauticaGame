using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    class StaticObject : GameObject
    {
        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation) : base(gameInstance, name, meshes)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;

            TGCMatrix scaleTransform = TGCMatrix.Scaling(Scale);
            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix rotationTransform = TGCMatrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

            Transform = scaleTransform * rotationTransform * translation;
        }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, float scale, TGCVector3 rotation)
            : this(gameInstance, name, meshes, position, TGCVector3.One * scale, rotation) { }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, float scale)
            : this(gameInstance, name, meshes, position, TGCVector3.One * scale, TGCVector3.Empty) { }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, TGCVector3 rotation)
            : this(gameInstance, name, meshes, position, TGCVector3.One, rotation) { }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position)
            : this(gameInstance, name, meshes, position, TGCVector3.One, TGCVector3.Empty) { }

        #region GameObject

        public override void Update() { /* No tienen logica */ }

        public override void Render()
        {
            foreach (TgcMesh mesh in Meshes)
            {
                mesh.Transform = Transform;
                mesh.Render();
            }
        }

        public override void Dispose()
        {
            foreach (TgcMesh mesh in Meshes)
                mesh.Dispose();
        }

        #endregion
    }
}
