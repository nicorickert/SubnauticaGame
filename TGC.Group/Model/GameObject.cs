using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model
{
    public abstract class GameObject
    {
        public TGCExample GameInstance { get; private set; }
        public string Name { get; private set; }
        public TgcMesh Mesh { get; protected set; }
        public TGCVector3 LookDirection { get; protected set; }
        public TGCVector3 Position
        {
            get { return Mesh.Position; }
            protected set
            {
                Position = value;
                Mesh.Position = value;
            }
        }


        public GameObject(TGCExample gameInstance, string name)
        {
            GameInstance = gameInstance;
            Name = name;
        }

        public abstract void Update();
        public abstract void Render();
        public abstract void Dispose();
    }
}
