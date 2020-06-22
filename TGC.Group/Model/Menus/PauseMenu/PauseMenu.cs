using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.PauseMenu
{
    class PauseMenu : Menu
    {
        private CustomBitmap bitmapSlotBackground;
        private Subnautica gameInstance;
        private CustomSprite background;
        private TgcText2D pauseText;
        public PauseMenu(Subnautica gameInstance)
            : base()
        {
            this.gameInstance = gameInstance;
            bitmapSlotBackground = new CustomBitmap(Game.Default.MediaDirectory + "craftingSlotBackground.jpg", D3DDevice.Instance.Device);

            background = new CustomSprite(Game.Default.MediaDirectory + "blackSquare.jpg");
            background.Color = Color.FromArgb(170,0,0,0); // para la transparencia
            background.SrcRect = new Rectangle(0, 0, D3DDevice.Instance.Width, D3DDevice.Instance.Height);
            background.Position = TGCVector2.Zero;

            pauseText = new TgcText2D();
            pauseText.Text = "Pausa";
            pauseText.Position = new Point(D3DDevice.Instance.Width / 2 - 100, D3DDevice.Instance.Height / 2 - 65);
            pauseText.Align = TgcText2D.TextAlign.LEFT;
            pauseText.changeFont(new Font("TimesNewRoman", 50, FontStyle.Bold));
            pauseText.Color = Color.DarkGray;
        }

        public override void Render()
        {
            if (gameInstance.FocusInGame)   // Solo renderizar si estoy en pausa
                return;
            base.Render();
            Drawer2D drawer = new Drawer2D();

            drawer.BeginDrawSprite();

            drawer.DrawSprite(background);

            drawer.EndDrawSprite();
            pauseText.render();
        }
        public override void Dispose()
        {
            background.Dispose();
            pauseText.Dispose();
        }

        public override void Update(float elapsedTime)
        {
            return;
        }

        public override void UpdateSlotDisplay()
        {
            return;
        }
    }
}
