using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model
{
    public abstract class GameObject
    {
        public Subnautica GameInstance { get; private set; }
        public string Name { get; private set; }
        public TgcMesh Mesh { get; protected set; }
        public TGCVector3 LookDirection { get; protected set; } = new TGCVector3(0, 0, -1);
        public TGCVector3 Position
        {
            get { return Mesh.Position; }
            protected set
            {
                Mesh.Position = value;
            }
        }

        public GameObject(Subnautica gameInstance, string name)
        {
            GameInstance = gameInstance;
            Name = name;
            GameInstance.InstanceObject(this);
        }

        public abstract void Update();
        public abstract void Render();
        public abstract void Dispose();
    }
}
