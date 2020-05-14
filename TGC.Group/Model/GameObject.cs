using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using System.Collections.Generic;
using TGC.Core.Collision;
using TGC.Core;
using System.Linq;
using System.Drawing;

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
        public List<TgcMesh> Meshes { get; protected set; }
        public TGCVector3 InitialLookDirection = new TGCVector3(0, 0, -1);
        public TGCVector3 LookDirection { get; set; }
        public TGCVector3 RelativeUpDirection
        {
            get
            {
                //Se busca el vector que es producto del (0,1,0)Up y la direccion de vista.
                TGCVector3 relativeXDirection = TGCVector3.Cross(TGCVector3.Up, LookDirection);
                //El vector de Up correcto dependiendo del LookDirection
                return TGCVector3.Cross(LookDirection, relativeXDirection);  // LookDirection sería como el relativeZDirection
            }
        }
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
            LookDirection = InitialLookDirection;
        }

        #region TGC

        public abstract void Update();

        public virtual void Render()
        {
            foreach (TgcMesh mesh in Meshes)
            {
                mesh.Render();
                mesh.BoundingBox.Render(); // Borrar para no mostrar los bounding box
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
            Dispose();
            GameInstance.DestroyObject(this);
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

        protected List<GameObject> ObjectsWithinRange(int range) => GameInstance.SceneObjects.FindAll(obj => obj != this && TGCVector3.Length(obj.Position - Position) <= range);

        protected bool CollisionDetected() => Meshes.Any(mesh => NearObjects().Any(obj => CollidesWith(obj)));

        protected void SimulateAndSetTransformation(TGCVector3 newPosition, TGCMatrix newTransform)
        {
            TGCVector3 oldPosition = Position;
            TGCMatrix oldTransform = Transform;

            Position = newPosition;
            Transform = newTransform;

            if (CollisionDetected())
            {
                Position = oldPosition;
                Transform = oldTransform;
            }
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

        #endregion
    }
}
