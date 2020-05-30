using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Menus.CraftingMenu
{
    class CraftingMenu
    {
        private TGCVector2 position;
        private List<CraftingSlot> craftingSlots = new List<CraftingSlot>();

        private Player _owner = null;
        public Player Owner
        {
            get => _owner;

            set
            {
                _owner = value;

                craftingSlots.Clear();

                TGCVector2 nextPosition = position;
                foreach (var bp in _owner.AvailableBluePrints)
                {
                    CraftingSlot craftingSlot = new CraftingSlot(bp, nextPosition);
                    craftingSlots.Add(craftingSlot);
                    nextPosition.Y += craftingSlot.Size.Height + 10;

                    // TODO ver si necesito hacer dos columnas
                }
            }
        }

        public CraftingMenu(TGCVector2 position)
        {
            this.position = position;
        }

        public void Update()
        {
            // Chequear seleccion
        }

        public void Render()
        {
            if (Owner != null)
            {
                foreach (var slot in craftingSlots)
                    slot.Render();
            }
        }

        public void Dispose()
        {
            foreach (var slot in craftingSlots)
                slot.Dispose();
        }
    }
}
