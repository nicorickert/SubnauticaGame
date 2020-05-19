using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public abstract class Roamer : LivingNPC
    {
        #region SETTINGS
        private float timeSinceLastDirection = 0f;
        private float timeToChangeDirection = 3f;
        #endregion

        protected float roamingSpeed;

        public Roamer(Subnautica gameInstance, string name, List<TgcMesh> meshes, int maxHealth, float roamingSpeed) : base(gameInstance, name, meshes, maxHealth)
        {
            this.roamingSpeed = roamingSpeed;
        }

        public virtual void Roam()
        {
            LookDirection = new TGCVector3(LookDirection.X, 0, LookDirection.Z);  // Elimino la componente Y para que roamee al nivel que esta del agua

            timeSinceLastDirection += GameInstance.ElapsedTime;

            TGCVector3 rotationVector = TGCVector3.Empty;

            if (timeSinceLastDirection > timeToChangeDirection)
            {
                timeSinceLastDirection = 0f;
                timeToChangeDirection = MathExtended.GetRandomNumberBetween(2, 5);

                float randomX = MathExtended.GetRandomNumberBetween(-100, 100);
                float randomZ = MathExtended.GetRandomNumberBetween(-100, 100);

                TGCVector3 lastDirection = LookDirection;
                LookDirection = TGCVector3.Normalize(new TGCVector3(randomX, LookDirection.Y, randomZ));

                float angle = MathExtended.AngleBetween(new TGCVector2(lastDirection.X, lastDirection.Z), new TGCVector2(LookDirection.X, LookDirection.Z));

                rotationVector.Y = -angle;
            }

            rotation += rotationVector;
            Position += LookDirection * roamingSpeed * GameInstance.ElapsedTime;

            TGCMatrix rot = TGCMatrix.RotationY(rotation.Y);
            TGCMatrix trans = TGCMatrix.Translation(Position);
            TGCMatrix scal = TGCMatrix.Scaling(scale);

            Transform = scal * rot * trans;
        }
    }
}
