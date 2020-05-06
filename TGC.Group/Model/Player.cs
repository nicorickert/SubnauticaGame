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
        private readonly float movementSpeed = 1000.0f;

        public Player(Subnautica gameInstance, string name, TgcMesh mesh) : base(gameInstance, name, mesh)
        {
            Mesh.Position = new TGCVector3(0, 100, 2000);
        }

        #region GameObject

        public override void Update()
        {
            nextTransform = TGCMatrix.Identity;
            FixRotation();
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
            else if (input.keyDown(Key.S))  // Atras
            {
                movementDirection -= LookDirection;
            }

            if (input.keyDown(Key.A))  // Izquierda
            {
                TGCQuaternion quat = TGCQuaternion.RotationAxis(RelativeUpDirection, FastMath.PI_HALF);
                TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(quat);
                movementDirection += MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }
            else if (input.keyDown(Key.D))  // Derecha
            {
                TGCQuaternion quat = TGCQuaternion.RotationAxis(RelativeUpDirection, FastMath.PI_HALF * 3);
                TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(quat);
                movementDirection += MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }

            if (input.keyDown(Key.Space))    // De acá para abajo son solo de prueba para poder moverme en la escena libremente
            {
                movementDirection += new TGCVector3(0, 1, 0);
            }
            else if (input.keyDown(Key.X))
            {
                movementDirection += new TGCVector3(0, -1, 0);
            }

            if (!CollisionDetected())
            {
                TGCVector3 totalTranslation = TGCVector3.Normalize(movementDirection) * movementSpeed * GameInstance.ElapsedTime;
                Mesh.Position += totalTranslation;

                TGCMatrix translationMatrix = TGCMatrix.Translation(Mesh.Position);
                nextTransform *= translationMatrix;
            }
        }

        private bool CollisionDetected() => false;  // TODO Tal vez podria ir en la clase GameObject

        private void FixRotation()
        {
            TGCVector3 rotationAxis = TGCVector3.Cross(InitialLookDirection, LookDirection);  // Ojo el orden - no es conmutativo
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(rotationAxis, MathExtended.AngleBetween(InitialLookDirection, LookDirection));
            TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(rotation);
            nextTransform *= rotationMatrix;  // TODO Ver cuando estén las colisiones si hay que hacer la rotacion respecto de la cabeza o desde los pies (actualmente desde los pies)
        }

        #endregion

        #region PUBLIC_METHODS


        #endregion
    }
}
