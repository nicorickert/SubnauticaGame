using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils.Sprites;

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

                if(_owner != null)
                {
                    TGCVector2 nextPosition = position;
                    foreach (var bp in _owner.AvailableBluePrints)
                    {
                        CraftingSlot craftingSlot = new CraftingSlot(bp, nextPosition);
                        craftingSlots.Add(craftingSlot);
                        nextPosition.Y += craftingSlot.Size.Height + 30;

                        // TODO ver si necesito hacer dos columnas
                    }
                }
            }
        }
        public bool IsBeingUsed { get => Owner != null; }

        public CraftingMenu()
        {
            float deviceHeight = D3DDevice.Instance.Height;
            float deviceWidth = D3DDevice.Instance.Width;

            position = new TGCVector2(deviceWidth * 0.3f, deviceHeight * 0.1f);
        }

        public void Update()
        {
            // Chequear seleccion
        }

        public void Render()
        {
            Drawer2D drawer = new Drawer2D();

            drawer.BeginDrawSprite();

            foreach (var slot in craftingSlots)
                slot.RenderSprites(drawer);

            drawer.EndDrawSprite();

            foreach (var slot in craftingSlots)
                slot.RenderText();
        }

        public void Dispose()
        {
            foreach (var slot in craftingSlots)
                slot.Dispose();
        }

        public void Open(Player crafter)
        {
            Owner = crafter;
        }

        public void Close()
        {
            Owner = null;
        }
    }
}
