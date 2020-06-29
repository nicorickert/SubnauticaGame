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
        private CustomSprite wasdSprite;
        private CustomSprite escISprite;
        private CustomSprite mouseSprite;
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

            wasdSprite = new CustomSprite(gameInstance.MediaDir + "Sprites//WASD.png");
            wasdSprite.SrcRect = new Rectangle(0, 0, D3DDevice.Instance.Width, D3DDevice.Instance.Height);
            wasdSprite.Position = new TGCVector2(D3DDevice.Instance.Width / 8, D3DDevice.Instance.Height / 2.5f);

            escISprite = new CustomSprite(gameInstance.MediaDir + "Sprites//EscI.png");
            escISprite.SrcRect = new Rectangle(0, 0, D3DDevice.Instance.Width, D3DDevice.Instance.Height);
            escISprite.Position = new TGCVector2(D3DDevice.Instance.Width / 2.7f, D3DDevice.Instance.Height / 2.5f);

            mouseSprite = new CustomSprite(gameInstance.MediaDir + "Sprites//Mouse.png");
            mouseSprite.SrcRect = new Rectangle(0, 0, D3DDevice.Instance.Width, D3DDevice.Instance.Height);
            mouseSprite.Position = new TGCVector2(D3DDevice.Instance.Width / 1.5f, D3DDevice.Instance.Height / 2f);

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
            drawer.DrawSprite(wasdSprite);
            drawer.DrawSprite(escISprite);
            drawer.DrawSprite(mouseSprite);

            drawer.EndDrawSprite();
            titleText.render();
        }
        public override void Dispose()
        {
            background.Dispose();
            titleText.Dispose();
            wasdSprite.Dispose();
            escISprite.Dispose();
            mouseSprite.Dispose();
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

