using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils;
using System;


namespace TGC.Group.Model
{
    class Fish : GameObject
    {
        private float movementSpeed = 100f;
        private float size;


        private TGCMatrix nextTransform = TGCMatrix.Identity;
        private float timeSinceLastDirection = 0f;
        private float timeToChangeDirection = 3f;


        public Fish(Subnautica gameInstance, string name, TGCVector3 spawnLocation) : base(gameInstance, name)
        {
            var loader = new TgcSceneLoader();
            Mesh = loader.loadSceneFromFile(gameInstance.MediaDir + "Aquatic\\Meshes\\fish-TgcScene.xml").Meshes[0];
            Position = spawnLocation;
            size = MathExtended.GetRandomNumberBetween(3, 20);
            Scale = TGCVector3.One * size;
        }

        public override void Update()
        {
            ManageMovement();
        }

        public override void Render()
        {
            Transform = nextTransform;
            Mesh.Render();
        }

        public override void Dispose()
        {
            Mesh.Dispose();
        }


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

            Rotation += rotationVector;
            Position += LookDirection * movementSpeed * GameInstance.ElapsedTime;

            TGCMatrix rotation = TGCMatrix.RotationY(Rotation.Y);
            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix scaling = TGCMatrix.Scaling(Scale);

            nextTransform = scaling *  rotation * translation;
        }
    }
}
