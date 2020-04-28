using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils;
using System;


namespace TGC.Group.Model
{
    class Fish : GameObject
    {
        private float movementSpeed = 10f;


        private TGCMatrix nextTransform = TGCMatrix.Identity;
        private float timeSinceLastDirection = 0f;
        private readonly int movingRadius = 1000;


        public Fish(Subnautica gameInstance, string name, TGCVector3 spawnLocation) : base(gameInstance, name)
        {
            var loader = new TgcSceneLoader();
            Mesh = loader.loadSceneFromFile(gameInstance.MediaDir + "Aquatic\\Meshes\\fish-TgcScene.xml").Meshes[0];
            Position = spawnLocation;
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

            if (timeSinceLastDirection > 3f)
            {
                timeSinceLastDirection = 0f;

                Random random = new Random();
                float randomX = random.Next();  // Son todas positivas
                float randomZ = random.Next();  // Son todas positivas

                TGCVector3 lastDirection = LookDirection;
                LookDirection = TGCVector3.Normalize(new TGCVector3(randomX, LookDirection.Y, randomZ));

                float angle = MathExtended.AngleBetween(new TGCVector2(lastDirection.X, lastDirection.Z), new TGCVector2(LookDirection.X, LookDirection.Z));

                rotationVector.Y += angle;
            }

            Rotation += rotationVector;
            Position += LookDirection * movementSpeed * GameInstance.ElapsedTime;

            TGCMatrix rotation = TGCMatrix.RotationYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            TGCMatrix translation = TGCMatrix.Translation(Position);
            
            nextTransform = rotation * translation;
        }
    }
}
