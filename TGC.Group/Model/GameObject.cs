﻿using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Example;

namespace TGC.Group.Model
{
    public abstract class GameObject
    {
        public Subnautica GameInstance { get; private set; }
        public string Name { get; private set; }
        public TgcMesh Mesh { get; protected set; }
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
        public TGCVector3 Position
        {
            get { return Mesh.Position; }
            protected set
            {
                Mesh.Position = value;
            }
        }
        public TGCVector3 Scale
        {
            get { return Mesh.Scale; }
            protected set
            {
                Mesh.Scale = value;
            }
        }
        public TGCVector3 Rotation
        {
            get { return Mesh.Rotation; }
            protected set
            {
                Mesh.Rotation = value;
            }
        }
        public TGCMatrix Transform
        {
            get { return Mesh.Transform; }
            protected set
            {
                Mesh.Transform = value;
            }
        }

        public GameObject(Subnautica gameInstance, string name, TgcMesh mesh)
        {
            GameInstance = gameInstance;
            Name = name;
            Mesh = mesh;
            LookDirection = InitialLookDirection;
        }

        public abstract void Update();
        public abstract void Render();
        public abstract void Dispose();
    }
}
