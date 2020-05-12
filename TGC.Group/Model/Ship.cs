using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Ship : GameObject
    {
        public Ship(Subnautica gameInstance, string name, List<TgcMesh> meshes) : base(gameInstance, name, meshes)
        {
            Position = new TGCVector3(3500, 60, 0);   // seteo la posicion del barco
            Scale *= 3;
            Rotation = new TGCVector3(0, FastMath.PI_HALF, 0);

            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix scaling = TGCMatrix.Scaling(Scale);
            TGCMatrix rotation = TGCMatrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);

            Transform = rotation * scaling * translation;
        }

        public override void Update() { /* Sin logica por ahora*/ }

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
    }
}
