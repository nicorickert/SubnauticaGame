using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    public class Player : GameObject
    {
        private TGCMatrix nextTransform = TGCMatrix.Identity;

        /* STATS */
        private float movementSpeed = 500.0f;
        private float angularVelocity = 2.0f;

        public Player(Subnautica gameInstance, string name) : base(gameInstance, name)
        {
            var loader = new TgcSceneLoader();
            var playerScene = loader.loadSceneFromFile(GameInstance.MediaDir + "Player\\Player-TgcScene.xml");
            Mesh = playerScene.Meshes[0];
            Mesh.Position = new TGCVector3(0, 100, 2000);
        }

        #region GameObject

        public override void Update()
        {
            ManageMovement();
        }

        public override void Render()
        {
            Mesh.Transform = nextTransform;
            Mesh.Render();
        }

        public override void Dispose()
        {
            Mesh.Dispose();
        }

        #endregion

        #region PRIVATE_METHODS

        // <summary>
        //      Dado un input WASD el Mesh se mueve a la izquierda, derecha, adelante y atras respecto del vector LookDirection.
        // <summary>
        private void ManageMovement()
        {
            TgcD3dInput input = GameInstance.Input;
            TGCVector3 movementDirection = TGCVector3.Empty;
            TGCVector3 rotationVector = TGCVector3.Empty;

            if (input.keyDown(Key.W))  // Adelante
            {
                movementDirection += LookDirection;
            }
            if (input.keyDown(Key.S))  // Atras
            {
                movementDirection -= LookDirection;
            }
            if (input.keyDown(Key.A))  // Izquierda
            {
                TGCMatrix rotationMatrix = TGCMatrix.RotationY(FastMath.PI_HALF);
                movementDirection +=   MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }
            if (input.keyDown(Key.D))  // Derecha
            {
                TGCMatrix rotationMatrix = TGCMatrix.RotationY(FastMath.PI_HALF * 3);
                movementDirection += MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }
            if (input.keyDown(Key.Space))    // De acá para abajo son solo de prueba para poder moverme en la escena libremente
            {
                movementDirection += new TGCVector3(0, 1, 0);
            }
            if (input.keyDown(Key.X))
            {
                movementDirection += new TGCVector3(0, -1, 0);
            }
            if (input.keyDown(Key.Q))
            {
                rotationVector += new TGCVector3(0, 1, 0);
            }
            if (input.keyDown(Key.E))
            {
                rotationVector += new TGCVector3(0, -1, 0);
            }

            if (!CollisionDetected())
            {
                TGCVector3 totalTranslation = movementDirection * movementSpeed * GameInstance.ElapsedTime;
                TGCVector3 totalRotation = rotationVector * angularVelocity * GameInstance.ElapsedTime;

                Mesh.Position += totalTranslation;
                Mesh.Rotation -= totalRotation;

                TGCMatrix rotacionRespectoDelMesh = TGCMatrix.RotationYawPitchRoll(totalRotation.Y, totalRotation.X, totalRotation.Z); // Esta rotacion debería ser respecto de el eje Y del mesh.
                LookDirection = MathExtended.TransformVector3(rotacionRespectoDelMesh, LookDirection);

                TGCMatrix translationMatrix = TGCMatrix.Translation(Mesh.Position);
                TGCMatrix rotationMatrix = TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z);
                nextTransform = rotationMatrix * translationMatrix;
            }
        }

        private bool CollisionDetected() => false;  // TODO Tal vez podria ir en la clase GameObject

        #endregion

        #region PUBLIC_METHODS


        #endregion
    }
}
