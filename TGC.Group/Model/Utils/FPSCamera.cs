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
            TGCVector3 camaraPosition = player.Position + eyePosition;
            SetCamera(camaraPosition, camaraPosition + player.LookDirection);
        }
    }
}
