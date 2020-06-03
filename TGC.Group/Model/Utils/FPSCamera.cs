using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;


namespace TGC.Group.Model.Utils
{
    class FPSCamera : TgcCamera
    {
        private Subnautica GameInstance;
        private GameObject player;
        private TGCVector3 eyePosition;
        private float mousePositionX = 0;
        private float mousePositionY = 0;
        private readonly float mouseSensibility = 0.015f;
        private readonly float maxUpDownView = FastMath.PI_HALF - 0.01f;

        public FPSCamera(Subnautica gameInstance, GameObject player, TGCVector3 eyePosition) : base()
        {
            this.player = player;
            this.eyePosition = eyePosition;
            this.GameInstance = gameInstance;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            /*SETEO LA CAMARA*/
            //float normEyePosition = TGCVector3.Length(eyePosition);
            //TGCVector3 eyePositionLookingTo = normEyePosition * TGCVector3.Normalize(player.LookDirection);  // Pongo la norma del eyePosition en la direccion del lookDir
            //TGCVector3 camaraPosition = player.Position + eyePositionLookingTo + player.RelativeUpDirection * eyePosition.Y;

            // ---
            if (GameInstance.MouseEnabled) // Si se tiene el mouse activado, no rotar la cámara
                return;

            TGCVector3 transformedRelativeEyePosition = eyePosition.X * player.RelativeRightDirection + eyePosition.Y * player.RelativeUpDirection + eyePosition.Z * player.LookDirection; 
            TGCVector3 camaraPosition = player.Position + transformedRelativeEyePosition;

            SetCamera(camaraPosition, camaraPosition + player.LookDirection);

            // ROTO EL LOOKDIRECTION DEL PLAYER
            UpdatePlayerLookDirection();
        }

        private void UpdatePlayerLookDirection()
        {
            TgcD3dInput input = player.GameInstance.Input;

            // MOVER LOOK DIRECTION CON MOVIMIENTO DEL MOUSE
            if (player.GameInstance.FocusInGame)
            {
                mousePositionY -= -input.XposRelative * mouseSensibility; // ENTENDER PORQUE TENGO QUE INVERTIR LAS X CON Y

                if (mousePositionX >= 0)                // Para que no se haga gimball lock en la camara, limito el maximo que se puede llegar hasta arriba y abajo
                {
                    mousePositionX = FastMath.Min(mousePositionX - input.YposRelative * mouseSensibility, maxUpDownView);
                }
                else
                {
                    mousePositionX = FastMath.Max(mousePositionX - input.YposRelative * mouseSensibility, -maxUpDownView);
                }
            }

            TGCMatrix cameraRotationMatrix = TGCMatrix.RotationX(mousePositionX) * TGCMatrix.RotationY(mousePositionY);
            player.LookDirection = TGCVector3.TransformNormal(player.InitialLookDirection, cameraRotationMatrix);
        }
    }
}
