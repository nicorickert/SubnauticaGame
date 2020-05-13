using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using System.Collections.Generic;
using TGC.Core.Collision;
using System.Linq;
using System.Drawing;

namespace TGC.Group.Model
{
    public abstract class GameObject
    {
        private TGCMatrix transform = TGCMatrix.Identity;

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
        public TGCVector3 Scale { get; set; } = TGCVector3.One;
        public TGCVector3 Rotation { get; set; } = TGCVector3.Empty;
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

        protected readonly int range = 600;

        public GameObject(Subnautica gameInstance, string name, List<TgcMesh> meshes)
        {
            GameInstance = gameInstance;
            Name = name;
            Meshes = meshes;
            LookDirection = InitialLookDirection;
        }


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


        public virtual void Interact(Player interactor) { System.Console.WriteLine(Name + " interacted with " + interactor.Name); }

        public void Destroy()
        {
            Dispose();
            GameInstance.DestroyObject(this);
        }

        public bool CheckRayCollision(TgcPickingRay pickingRay)
        {
            return Meshes.Any(mesh => TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, mesh.BoundingBox, out TGCVector3 collisionPoint));
        }

        public bool CollidesWith(TgcMesh foreignMesh)
        {
            return Meshes.Any(mesh => TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, foreignMesh.BoundingBox) != TgcCollisionUtils.BoxBoxResult.Afuera);
        }


        protected List<GameObject> ReachableObjects() => GameInstance.SceneObjects.FindAll(obj => obj != this && TGCVector3.Length(obj.Position - Position) <= range);

        protected bool CollisionDetected()
        {
            return Meshes.Any(mesh => ReachableObjects().Any(obj => obj.CollidesWith(mesh)));
        }
    }
}
