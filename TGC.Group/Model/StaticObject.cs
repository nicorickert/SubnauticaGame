using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    public class StaticObject : GameObject
    {
        public bool Enabled { get; set; } = false;

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation) : base(gameInstance, name, meshes)
        {
            CollisionStatus = Utils.ECollisionStatus.NOT_COLLISIONABLE;

            Position = position;
            this.rotation = rotation;
            this.scale = scale;

            TGCMatrix scaleTransform = TGCMatrix.Scaling(this.scale);
            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix rotationTransform = TGCMatrix.RotationYawPitchRoll(this.rotation.Y, this.rotation.X, this.rotation.Z);

            Transform = scaleTransform * rotationTransform * translation;
        }

        #region CONSTRUCTORES

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, float scale, TGCVector3 rotation)
            : this(gameInstance, name, meshes, position, TGCVector3.One * scale, rotation) { }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, float scale)
            : this(gameInstance, name, meshes, position, TGCVector3.One * scale, TGCVector3.Empty) { }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, TGCVector3 rotation)
            : this(gameInstance, name, meshes, position, TGCVector3.One, rotation) { }

        public StaticObject(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position)
            : this(gameInstance, name, meshes, position, TGCVector3.One, TGCVector3.Empty) { }

        #endregion

        #region TGC

        public override void Update() { /* No tienen logica */ }

        #endregion

        protected override List<TgcMesh> MeshesToRender()
        {
            if (Enabled)
                return Meshes;
            else
                return new List<TgcMesh>();
        }
    }
}
