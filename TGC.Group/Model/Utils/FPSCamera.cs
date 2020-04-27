using TGC.Core.Camara;
using TGC.Core.Mathematica;


namespace TGC.Group.Model.Utils
{
    class FPSCamera : TgcCamera
    {
        private GameObject player;
        private TGCVector3 eyePosition;

        public FPSCamera(GameObject player, TGCVector3 eyePosition) : base()
        {
            this.player = player;
            this.eyePosition = eyePosition;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            //TGCVector3 camaraPosition = player.Position + eyePosition;

            float normEyePosition = MathExtended.NormOfVector3(eyePosition);
            TGCVector3 eyePositionLookingTo = normEyePosition * TGCVector3.Normalize(player.LookDirection);  // Pongo la norma del eyePosition en la direccion del lookDir
            TGCVector3 camaraPosition = player.Position + eyePositionLookingTo + new TGCVector3(0, eyePosition.Y, 0);

            SetCamera(camaraPosition, camaraPosition + player.LookDirection);
        }
    }
}
