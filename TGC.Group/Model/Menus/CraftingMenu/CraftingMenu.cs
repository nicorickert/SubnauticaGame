using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.CraftingMenu
{
    class CraftingMenu : Menu
    {
        private bool craftingCooldownEnabled = false;

        public Player Owner => owner;

        public CraftingMenu() 
            : base() { }

        public override void Update(float elapsedTime)
        {
            if (craftingCooldownEnabled)
                timeSinceLastClick += elapsedTime;

            if (IsBeingUsed)
            {
                CheckClicks();
            }
        }

        public override void Open(Player crafter)
        {
            base.Open(crafter);

            UpdateSlotDisplay();
            timeSinceLastClick = 0f;
            craftingCooldownEnabled = true;
        }

        public override void Close()
        {
            base.Close();

            craftingCooldownEnabled = false;
        }

        protected override void UpdateSlotDisplay()
        {
            slots.Clear();

            TGCVector2 nextPosition = position;
            foreach (var bp in owner.AvailableBluePrints)
            {
                CraftingSlot craftingSlot = new CraftingSlot(nextPosition, bp);
                slots.Add(craftingSlot);
                nextPosition.Y += craftingSlot.Size.Height + 30;
            }
        }
    }
}
