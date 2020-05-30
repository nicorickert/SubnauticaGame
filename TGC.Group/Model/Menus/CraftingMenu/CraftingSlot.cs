using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Items;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.CraftingMenu
{
    class CraftingSlot
    {
        private CustomSprite productSlot;
        private CustomSprite productSprite;
        private TgcText2D title = new TgcText2D();
        private TgcText2D description = new TgcText2D();

        public Size Size { get => new Size(description.Size.Width - (int)productSlot.Position.X, productSlot.Bitmap.Size.Height); }

        public CraftingSlot(BluePrint bluePrint, TGCVector2 position)
        {
            productSlot = new CustomSprite(Game.Default.TexturaCaja);
            productSlot.Position = position;

            productSprite = new CustomSprite(ItemDatabase.Instance.ItemsInfo[bluePrint.ProductId].SpritePath);
            productSprite.Position = position + new TGCVector2(productSlot.Bitmap.Size.Width / 4, productSlot.Bitmap.Size.Height / 4);

            title.Text = ItemDatabase.Instance.ItemsInfo[bluePrint.ProductId].Name;
            title.Position = new Point((int) position.X + productSlot.Bitmap.Size.Width + 10, (int) position.Y);

            description.Text = bluePrint.Description;
            description.Position = new Point(title.Position.X, title.Position.Y - 10); 
        }


        public void Render()
        {
            // TODO
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
