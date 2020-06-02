using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Items;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.CraftingMenu
{
    class CraftingSlot
    {
        private CustomSprite slotBackground;
        private CustomSprite productSlot;
        private CustomSprite productSprite;
        private TgcText2D title = new TgcText2D();
        private TgcText2D description = new TgcText2D();

        public Size Size { get; private set; }

        public CraftingSlot(BluePrint bluePrint, TGCVector2 position)
        {
            Item productSample = ItemDatabase.Instance.Generate(bluePrint.ProductId);

            productSlot = new CustomSprite(Game.Default.MediaDirectory + "cajaMadera4.jpg");
            productSlot.Position = new TGCVector2(position.X + 10, position.Y + 10);
            float slotScalingFactor = 0.5f;
            productSlot.Scaling = TGCVector2.One * slotScalingFactor;

            productSprite = new CustomSprite(productSample.SpritePath);
            productSprite.Scaling = TGCVector2.One * 0.25f;
            productSprite.Position = productSlot.Position + new TGCVector2(productSlot.Bitmap.Size.Width * slotScalingFactor / 4, productSlot.Bitmap.Size.Height * slotScalingFactor / 4);

            title.Text = productSample.Name;
            title.Position = new Point((int) (productSlot.Position.X + productSlot.Bitmap.Size.Width * slotScalingFactor + 15), (int) productSlot.Position.Y + 20);
            title.Align = TgcText2D.TextAlign.LEFT;
            title.changeFont(new Font("TimesNewRoman", 20, FontStyle.Bold));
            title.Color = Color.DarkGray;

            description.Text = bluePrint.Description;
            description.Position = new Point(title.Position.X, title.Position.Y + 30);
            description.Size = new Size(600, 300);
            description.Align = TgcText2D.TextAlign.LEFT;
            description.changeFont(new Font("TimesNewRoman", 14, FontStyle.Bold));
            description.Color = Color.LightGray;

            Size = new Size((int)(productSlot.Bitmap.Size.Width * slotScalingFactor + description.Size.Width + 30), (int)(productSlot.Bitmap.Height * slotScalingFactor + 20));

            slotBackground = new CustomSprite(Game.Default.MediaDirectory + "craftingSlotBackground.jpg");
            slotBackground.Position = position;
            slotBackground.SrcRect = new Rectangle((int)position.X, (int)position.Y, Size.Width, Size.Height);
        }


        public void RenderSprites(Drawer2D drawer)
        {
            drawer.DrawSprite(slotBackground);
            drawer.DrawSprite(productSlot);
            drawer.DrawSprite(productSprite);
        }

        public void RenderText()
        {
            title.render();
            description.render();
        }

        public void Dispose()
        {
            productSlot.Dispose();
            productSprite.Dispose();
            title.Dispose();
            description.Dispose();
        }
    }
}
