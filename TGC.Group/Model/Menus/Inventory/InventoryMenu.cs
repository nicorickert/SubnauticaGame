using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.Inventory
{
    class InventoryMenu : Menu
    {
        private CustomBitmap bitmapSlotBackground;
        public InventoryMenu(Player owner)
            : base() {
            bitmapSlotBackground = new CustomBitmap(Game.Default.MediaDirectory + "craftingSlotBackground.jpg", D3DDevice.Instance.Device);
        }

        public override void Update(float elapsedTime)
        {
            if (IsBeingUsed)
            {
                timeSinceLastClick += elapsedTime;
                //UpdateSlotDisplay();
                CheckClicks();
            }
        }

        public override void Open(Player user)
        {
            base.Open(user);

            UpdateSlotDisplay();
        }

        public override void UpdateSlotDisplay()
        {
            slots.Clear();

            TGCVector2 nextPosition = position;
            foreach (var itemID in owner.Inventory.AccumulatedItems.Keys)
            {
                InventorySlot slot = new InventorySlot(nextPosition, itemID, owner.Inventory.AccumulatedItems[itemID], this, bitmapSlotBackground);
                slots.Add(slot);
                nextPosition.Y += slot.Size.Height + 30;
            }       
        }
    }
}
