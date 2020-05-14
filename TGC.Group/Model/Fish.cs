using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using System.Collections.Generic;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    class Fish : GameObject
    {
        #region STATS
        private float movementSpeed = 100f;
        private float size;
        #endregion

        #region UTILS
        private TGCMatrix nextTransform = TGCMatrix.Identity;
        private float timeSinceLastDirection = 0f;
        private float timeToChangeDirection = 3f;
        #endregion

        public Fish(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 spawnLocation) : base(gameInstance, name, meshes)
        {
            CollisionStatus = Utils.ECollisionStatus.NOT_COLLISIONABLE;
            Position = spawnLocation;
            size = MathExtended.GetRandomNumberBetween(3, 20);
            scale = TGCVector3.One * size;
        }

        #region TGC
        public override void Update()
        {
            ManageMovement();
        }
        #endregion

        #region INTERFACE
        public override void Interact(Player interactor)
        {
            interactor.CollectItem(new Food("raw_fish", "un spritepath", 20)); // ojo, arreglar lo del sprite path
            Destroy();
        }
        #endregion

        #region PRIVATE_METHODS

        private void ManageMovement()
        {
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
            Position += LookDirection * movementSpeed * GameInstance.ElapsedTime;

            TGCMatrix rot = TGCMatrix.RotationY(rotation.Y);
            TGCMatrix trans = TGCMatrix.Translation(Position);
            TGCMatrix scal = TGCMatrix.Scaling(scale);

            Transform = scal * rot * trans;
        }

        #endregion
    }
}
