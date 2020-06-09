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
    class InventoryMenu
    {
        private readonly float itemUseCooldown = 0.3f;
        private float timeSinceLastItemUse = 0f;

        private Player owner;
        private List<InventorySlot> slots = new List<InventorySlot>();
        private TGCVector2 position;

        public bool IsBeingUsed { get; set; } = false;

        public InventoryMenu(Player owner)
        {
            this.owner = owner;

            float deviceHeight = D3DDevice.Instance.Height;
            float deviceWidth = D3DDevice.Instance.Width;

            position = new TGCVector2(deviceWidth * 0.3f, deviceHeight * 0.1f);
        }


        public void Update(float elapsedTime)
        {
            if (IsBeingUsed)
            {
                UpdateSlots();

                timeSinceLastItemUse += elapsedTime;
                TgcD3dInput input = owner.GameInstance.Input;
                if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT) && timeSinceLastItemUse >= itemUseCooldown)
                {
                    TGCVector2 clickPosition = new TGCVector2(input.Xpos, input.Ypos);

                    InventorySlot selectedSlot = slots.Find(slot => slot.IsSelected(clickPosition));

                    if (selectedSlot != null)
                        selectedSlot.OnClick(owner);
                    else
                        Close();

                    timeSinceLastItemUse = 0;
                }
            }
        }

        public void Render()
        {
            Drawer2D drawer = new Drawer2D();

            drawer.BeginDrawSprite();

            foreach (var slot in slots)
                slot.RenderSprites(drawer);

            drawer.EndDrawSprite();

            foreach (var slot in slots)
                slot.RenderText();
        }

        public void Dispose()
        {
            foreach (var slot in slots)
                slot.Dispose();
        }

        public void UpdateSlots()
        {
            slots = new List<InventorySlot>();

            TGCVector2 nextPosition = position;
            foreach (var itemID in owner.Inventory.AccumulatedItems.Keys)
            {
                InventorySlot slot = new InventorySlot(nextPosition, itemID, owner.Inventory.AccumulatedItems[itemID]);
                slots.Add(slot);

                nextPosition.Y += slot.Size.Height + 30;
            }       
        }

        public void Open() { IsBeingUsed = true; owner.GameInstance.MouseEnable(); }
        public void Close() { IsBeingUsed = false; owner.GameInstance.MouseDisable(); }

    }
}
