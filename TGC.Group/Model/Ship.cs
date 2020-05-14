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
            scale *= 3;
            rotation = new TGCVector3(0, FastMath.PI_HALF, 0);

            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix scaling = TGCMatrix.Scaling(scale);
            TGCMatrix rot = TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

            Transform = rot * scaling * translation;
        }

        #region TGC
        public override void Update() { /* Sin logica por ahora*/ }
        #endregion
    }
}
