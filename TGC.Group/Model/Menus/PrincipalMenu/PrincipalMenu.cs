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

namespace TGC.Group.Model.Menus.PrincipalMenu
{
    class PrincipalMenu : Menu
    {
        private CustomBitmap bitmapSlotBackground;
        private Subnautica gameInstance;
        private CustomSprite background;
        private TgcText2D titleText;
        public PrincipalMenu(Subnautica gameInstance)
            : base()
        {
            input = gameInstance.Input;

            this.gameInstance = gameInstance;
            bitmapSlotBackground = new CustomBitmap(Game.Default.MediaDirectory + "craftingSlotBackground.jpg", D3DDevice.Instance.Device);

            background = new CustomSprite(Game.Default.MediaDirectory + "blackSquare.jpg");
            background.Color = Color.FromArgb(80, 0, 0, 0); // para la transparencia
            background.SrcRect = new Rectangle(0, 0, D3DDevice.Instance.Width, D3DDevice.Instance.Height);
            background.Position = TGCVector2.Zero;

            titleText = new TgcText2D();
            titleText.Text = "Tgcito se va de buceo";
            titleText.changeFont(new Font("TimesNewRoman", 50, FontStyle.Bold));
            titleText.Align = TgcText2D.TextAlign.CENTER;
            titleText.Position = new Point(0, D3DDevice.Instance.Height / 6);
            titleText.Color = Color.AntiqueWhite;

            UpdateSlotDisplay();
        }

        public override void Render()
        {
            if (!gameInstance.InPrincipalMenu)   // Solo renderizar si estoy en pausa
                return;
            base.Render();
            Drawer2D drawer = new Drawer2D();

            drawer.BeginDrawSprite();

            drawer.DrawSprite(background);

            drawer.EndDrawSprite();
            titleText.render();
        }
        public override void Dispose()
        {
            background.Dispose();
            titleText.Dispose();
        }

        public override void Update(float elapsedTime)
        {
            if (gameInstance.InPrincipalMenu)
            {
                timeSinceLastClick += elapsedTime;
                CheckClicks();
            }
        }

        public override void Open(Player user)
        {
            return;
        }

        public override void Close()
        {
            return;
        }

        public override void UpdateSlotDisplay()
        {
            TGCVector2 pos = new TGCVector2(D3DDevice.Instance.Width / 2 - 150, D3DDevice.Instance.Height / 3);
            PrincipalMenuSlot slot = new PrincipalMenuSlot(pos, this, "Comenzar juego", gameInstance.startGame, bitmapSlotBackground);
            slots.Add(slot);
        }
    }
}

