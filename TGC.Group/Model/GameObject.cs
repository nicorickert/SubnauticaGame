using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    public abstract class GameObject
    {
        public Subnautica GameInstance { get; private set; }
        public string Name { get; private set; }
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
        public TGCMatrix Transform { get; set; } = TGCMatrix.Identity;

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
                mesh.Transform = Transform;
                mesh.Render();
            }
        }

        public virtual void Dispose()
        {
            foreach (TgcMesh mesh in Meshes)
                mesh.Dispose();
        }
    }
}
