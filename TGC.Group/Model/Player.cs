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
        private float movementSpeed = 1000.0f;
        private float angularVelocity = 2.0f;

        private float mousePositionX = 0;
        private float prevMousePositionX = 0;
        private float mousePositionY = 0;
        private float mouseSensibility = 0.1f;
        private float MaxUpDownView = FastMath.PI_HALF - 0.01f;

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
            //Se busca el vector que es producto del (0,1,0)Up y la direccion de vista.
            TGCVector3 crossDirection = TGCVector3.Cross(TGCVector3.Up, LookDirection);
            //El vector de Up correcto dependiendo del LookDirection
            TGCVector3 upVector = TGCVector3.Cross(LookDirection, crossDirection);

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
                TGCQuaternion quat = TGCQuaternion.RotationAxis(upVector, FastMath.PI_HALF);
                TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(quat);
                movementDirection += MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }
            if (input.keyDown(Key.D))  // Derecha
            {
                TGCQuaternion quat = TGCQuaternion.RotationAxis(upVector, FastMath.PI_HALF * 3);
                TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(quat);
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



            // MOVER LOOK DIRECTION CON MOVIMIENTO DEL MOUSE
            if (GameInstance.focusInGame)
            {
                mousePositionY -= -input.XposRelative * mouseSensibility; // ENTENDER PORQUE TENGO QUE INVERTIR LAS X CON Y
  
                if (mousePositionX >= 0)                // Para que no se haga gimball lock en la camara, limito el maximo que se puede llegar hasta arriba y abajo
                {
                    mousePositionX = FastMath.Min(mousePositionX - input.YposRelative * mouseSensibility, MaxUpDownView);
                } else
                {
                    mousePositionX = FastMath.Max(mousePositionX - input.YposRelative * mouseSensibility, -MaxUpDownView);
                }
            }

            TGCMatrix cameraRotationMatrix = TGCMatrix.RotationX(mousePositionX) * TGCMatrix.RotationY(mousePositionY);
            LookDirection = TGCVector3.TransformNormal(InitialLookDirection, cameraRotationMatrix);
            

            if (!CollisionDetected())
            {
                TGCVector3 totalTranslation = movementDirection * movementSpeed * GameInstance.ElapsedTime;
                //TGCVector3 totalRotation = rotationVector * angularVelocity * GameInstance.ElapsedTime;

                Mesh.Position += totalTranslation;
                //Mesh.Rotation -= totalRotation;

                //TGCMatrix rotacionRespectoDelMesh = TGCMatrix.RotationYawPitchRoll(totalRotation.Y, totalRotation.X, totalRotation.Z); // Esta rotacion debería ser respecto de el eje Y del mesh.
                //LookDirection = MathExtended.TransformVector3(rotacionRespectoDelMesh, LookDirection);

                TGCMatrix translationMatrix = TGCMatrix.Translation(Mesh.Position);
                //TGCMatrix rotationMatrix = TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z);
                nextTransform = translationMatrix;
            }
        }

        private bool CollisionDetected() => false;  // TODO Tal vez podria ir en la clase GameObject

        #endregion

        #region PUBLIC_METHODS


        #endregion
    }
}
