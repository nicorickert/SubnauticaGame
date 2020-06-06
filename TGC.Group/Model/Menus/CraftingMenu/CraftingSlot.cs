using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Items;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.CraftingMenu
{
    class CraftingSlot : MenuSlot
    {
        private BluePrint bluePrint;

        private CustomSprite slotBackground;
        private CustomSprite productSlot;
        private CustomSprite productSprite;
        private TgcText2D title = new TgcText2D();
        private TgcText2D description = new TgcText2D();

        public CraftingSlot(TGCVector2 position, BluePrint bluePrint)
           : base(position)
        {
            Item productSample = ItemDatabase.Instance.Generate(bluePrint.ProductId);

            this.bluePrint = bluePrint;

            productSlot = new CustomSprite(Game.Default.MediaDirectory + "cajaMadera4.jpg");
            productSlot.Position = new TGCVector2(position.X + 10, position.Y + 10);
            float slotScalingFactor = 0.5f;
            productSlot.Scaling = TGCVector2.One * slotScalingFactor;

            productSprite = ItemDatabase.Instance.ItemSprites[productSample.ID];
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
            slotBackground.Position = Position;
            slotBackground.SrcRect = new Rectangle((int)position.X, (int)position.Y, Size.Width, Size.Height);
        }


        public override void RenderSprites(Drawer2D drawer)
        {
            drawer.DrawSprite(slotBackground);
            drawer.DrawSprite(productSlot);
            drawer.DrawSprite(productSprite);
        }

        public override void RenderText()
        {
            title.render();
            description.render();
        }

        public override void Dispose()
        {
            productSlot.Dispose();
            title.Dispose();
            description.Dispose();
        }

        public override void OnClick(Player clicker)
        {
            bluePrint.Craft(clicker);
        }
    }
}
