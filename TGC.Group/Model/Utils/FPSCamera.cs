using TGC.Core.Camara;
using TGC.Core.Mathematica;


namespace TGC.Group.Model.Utils
{
    class FPSCamera : TgcCamera
    {
        private GameObject player;
        private TGCVector3 relativePosition;

        public FPSCamera(GameObject player, TGCVector3 relativePosition) : base()
        {
            this.player = player;
            this.relativePosition = relativePosition;
        }

        public override void UpdateCamera(float elapsedTime)
        {
            TGCVector3 camaraPosition = player.Position + relativePosition;
            SetCamera(camaraPosition, camaraPosition + player.LookDirection, TGCVector3.Up);
        }
    }
}
