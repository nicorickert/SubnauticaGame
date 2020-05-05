using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;


namespace TGC.Group.Model.Utils
{
    class FPSCamera : TgcCamera
    {
        private GameObject player;
        private TGCVector3 eyePosition;
        private float mousePositionX = 0;
        private float prevMousePositionX = 0;
        private float mousePositionY = 0;
        private readonly float mouseSensibility = 0.01f;
        private readonly float maxUpDownView = FastMath.PI_HALF - 0.01f;

        public FPSCamera(GameObject player, TGCVector3 eyePosition) : base()
        {
            this.player = player;
            this.eyePosition = eyePosition;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            // ROTO EL LOOKDIRECTION DEL PLAYER

            TgcD3dInput input = player.GameInstance.Input;

            //Se busca el vector que es producto del (0,1,0)Up y la direccion de vista.
            TGCVector3 crossDirection = TGCVector3.Cross(TGCVector3.Up, player.LookDirection);
            //El vector de Up correcto dependiendo del LookDirection
            TGCVector3 upVector = TGCVector3.Cross(player.LookDirection, crossDirection);

            // MOVER LOOK DIRECTION CON MOVIMIENTO DEL MOUSE
            if (player.GameInstance.focusInGame)
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
            player.LookDirection = TGCVector3.TransformNormal(player.LookDirection, cameraRotationMatrix);



            // SETEO LA CAMARA

            //TGCVector3 camaraPosition = player.Position + eyePosition;
            float normEyePosition = TGCVector3.Length(eyePosition);
            TGCVector3 eyePositionLookingTo = normEyePosition * TGCVector3.Normalize(player.LookDirection);  // Pongo la norma del eyePosition en la direccion del lookDir
            TGCVector3 camaraPosition = player.Position + eyePositionLookingTo + new TGCVector3(0, eyePosition.Y, 0);

            SetCamera(camaraPosition, camaraPosition + player.LookDirection);
        }
    }
}
