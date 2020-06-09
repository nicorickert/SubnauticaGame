using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils.Sprites;
using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Menus.Inventory
{
    class InventoryMenu : Menu
    {
        private readonly float itemUseCooldown = 0.3f;
        private float timeSinceLastItemUse = 0f;

        private Player owner;
        private List<InventorySlot> slots = new List<InventorySlot>();
        private TGCVector2 position;

        public bool IsBeingUsed { get; set; } = false;

        public InventoryMenu(Player owner)
            : base() { }

        public override void Update(float elapsedTime)
        {
            if (IsBeingUsed)
            {
                timeSinceLastClick += elapsedTime;
                UpdateSlotDisplay();
                CheckClicks();
            }
        }

        protected override void UpdateSlotDisplay()
        {
            slots.Clear();

            TGCVector2 nextPosition = position;
            foreach (var itemID in owner.Inventory.AccumulatedItems.Keys)
            {
                InventorySlot slot = new InventorySlot(nextPosition, itemID, owner.Inventory.AccumulatedItems[itemID]);
                slots.Add(slot);
                nextPosition.Y += slot.Size.Height + 30;
            }       
        }
    }
}
