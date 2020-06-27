using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.PrincipalMenu
{
    class PrincipalMenuSlot : MenuSlot
    {
        TgcText2D text;
        Func<bool> onClickFunction;

        CustomSprite slotBackground;


        public PrincipalMenuSlot(TGCVector2 position, Menu menu, string text, Func<bool> onClickFunction , CustomBitmap bitmapBackground) : base(position, menu)
        {
            this.onClickFunction = onClickFunction;

            Size = new Size(300, 100);

            this.text = new TgcText2D();
            this.text.Text = text;
            this.text.Position = new Point((int)(position.X) + 50, (int)position.Y + 15 );
            this.text.Align = TgcText2D.TextAlign.LEFT;
            this.text.changeFont(new Font("TimesNewRoman", 20, FontStyle.Bold));
            this.text.Color = Color.White;


            slotBackground = new CustomSprite();
            slotBackground.Bitmap = bitmapBackground;
            slotBackground.Position = new TGCVector2(Position.X, Position.Y - 15);
            slotBackground.SrcRect = new Rectangle(0, 0,Size.Width, Size.Height);
        }

        public override void Dispose()
        {
            slotBackground.Dispose();
            text.Dispose();
        }

        public override void OnClick(Player clicker)
        {
            onClickFunction();
        }

        public override void RenderSprites(Drawer2D drawer)
        {
            drawer.DrawSprite(slotBackground);
        }

        public override void RenderText()
        {
            text.render();
        }
    }
}
