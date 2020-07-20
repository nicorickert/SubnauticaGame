using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using System.Collections.Generic;
using TGC.Core.Collision;
using TGC.Core;
using System.Linq;
using System.Drawing;
using TGC.Group.Model.Utils;
using TGC.Core.BoundingVolumes;
using TGC.Core.Shaders;
using TGC.Group.Model.Materials;

namespace TGC.Group.Model
{
    public abstract class GameObject
    {
        private TGCMatrix transform = TGCMatrix.Identity;
        protected TGCVector3 scale = TGCVector3.One;
        protected TGCVector3 rotation = TGCVector3.Empty;

        private readonly int  nearObjectsRange = 2000;

        #region PROPERTIES
        public Subnautica GameInstance { get; protected set; }
        public string Name { get; protected set; }
        public virtual Material MaterialInfo => Material.Opaque;
        public ECollisionStatus CollisionStatus { get; protected set; } = ECollisionStatus.COLLISIONABLE;
        public bool Collisionable { get { return CollisionStatus == ECollisionStatus.COLLISIONABLE; } }
        public List<TgcMesh> Meshes { get; protected set; }
        public TGCVector3 InitialLookDirection = new TGCVector3(0, 0, -1);
        public TGCVector3 LookDirection { get; set; }
        public TGCVector3 RelativeUpDirection { get { return TGCVector3.Normalize(TGCVector3.Cross(RelativeRightDirection, LookDirection)); /* LookDirection sería como el relativeForwardDirection*/ } }
        public TGCVector3 RelativeRightDirection { get { return TGCVector3.Normalize(TGCVector3.Cross(LookDirection, TGCVector3.Up)); } }
        public TGCVector3 Position { get; set; } = TGCVector3.Empty;
        public TGCMatrix Transform
        {
            get { return transform; }
            set
            {
                foreach (var mesh in Meshes)
                {
                    mesh.Transform = value;
                    mesh.BoundingBox.transform(value);
                    transform = value;
                }
            }
        }
        #endregion

        public GameObject(Subnautica gameInstance, string name, List<TgcMesh> meshes)
        {
            GameInstance = gameInstance;
            Name = name;
            Meshes = meshes;

            foreach (var mesh in Meshes)
            {
                GameInstance.loadEffectWithFogValues(mesh.Effect);
                mesh.Technique = "BlinnPhongTextured";
            }

            LookDirection = InitialLookDirection;
        }

        #region TGC

        public virtual void Update() { }

        public virtual void Render()
        {
            foreach (TgcMesh mesh in MeshesToRender())
            {
                mesh.Effect.SetValue("lightPosition", TGCVector3.TGCVector3ToFloat3Array(GameInstance.LightPosition));
                mesh.Effect.SetValue("eyePosition", TGCVector3.TGCVector3ToFloat3Array(GameInstance.Camera.Position));
                mesh.Effect.SetValue("ka", MaterialInfo.Ka);
                mesh.Effect.SetValue("kd", MaterialInfo.Kd);
                mesh.Effect.SetValue("ks", MaterialInfo.Ks);
                mesh.Render();
                
                if (GameInstance.RenderBB) // Para debug
                {
                    mesh.BoundingBox.Render();
                }
            }
        }

        public virtual void Dispose()
        {
            foreach (TgcMesh mesh in Meshes)
                mesh.Dispose();
        }

        #endregion

        #region INTERFACE
        public virtual void Interact(Player interactor) { System.Console.WriteLine(Name + " interacted with " + interactor.Name); }

        public void Destroy()
        {
            GameInstance.DestroyObject(this);
            Dispose();
        }

        #region COLLISIONS
        public bool CheckRayCollision(TgcPickingRay pickingRay)
        {
            return Meshes.Any(mesh => TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, mesh.BoundingBox, out TGCVector3 collisionPoint));
        }

        public bool CollidesWith(GameObject foreign)
        {
            return Meshes.Any(mesh => foreign.Meshes.Any(foreignMesh => TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, foreignMesh.BoundingBox) != TgcCollisionUtils.BoxBoxResult.Afuera));
        }
        #endregion
        #endregion

        #region PROTECTED
        protected List<GameObject> NearObjects() => ObjectsWithinRange(nearObjectsRange);

        protected List<GameObject> ObjectsWithinRange(int range) => GameInstance.SceneObjects.FindAll(obj => obj != this && IsWithinRange(range, obj));

        protected bool IsWithinRange(int range, GameObject obj) => TGCVector3.Length(obj.Position - Position) <= range;

        protected bool CollisionDetected() => Meshes.Any(mesh => NearObjects().FindAll(obj => obj.Collisionable).Any(obj => CollidesWith(obj)));

        protected void SimulateAndSetTransformation(TGCVector3 newPosition, TGCMatrix newTransform)
        {
            TGCVector3 oldPosition = Position;
            TGCMatrix oldTransform = Transform;

            Position = newPosition;
            Transform = newTransform;

            if (CollisionDetected() || OutOfBoundaries())
            {
                Position = oldPosition;
                Transform = oldTransform;
            }
        }

        protected bool OutOfBoundaries()
        {
            float radius = GameInstance.SueloDelMar.XZRadius;
            return FastMath.Abs(Position.X) > radius || FastMath.Abs(Position.Z) > radius;
        }

        protected void SimulateAndSetTransformation(TGCVector3 newPosition, TGCVector3 newRotation, TGCVector3 newScale, TGCMatrix newTransform)
        {
            TGCVector3 oldPosition = Position;
            TGCVector3 oldRotation = rotation;
            TGCVector3 oldScale = scale;
            TGCMatrix oldTransform = newTransform;

            Position = newPosition;
            rotation = newRotation;
            scale = newScale;
            Transform = newTransform;

            if (CollisionDetected())
            {
                Position = oldPosition;
                rotation = oldRotation;
                scale = oldScale;
                Transform = oldTransform;
            }
        }

        protected virtual List<TgcMesh> MeshesToRender()
        {
            // FRUSTUM CULLING
        
            return Meshes.FindAll(mesh =>
            {
                TgcCollisionUtils.FrustumResult collisionResult = TgcCollisionUtils.classifyFrustumAABB(GameInstance.Frustum, mesh.BoundingBox);
                return collisionResult != TgcCollisionUtils.FrustumResult.OUTSIDE;
            });
        }
        #endregion
    }
}
