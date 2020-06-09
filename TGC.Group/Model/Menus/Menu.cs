using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus
{
    public abstract class Menu
    {
        protected Player owner = null;
        protected TGCVector2 position;
        protected List<MenuSlot> slots = new List<MenuSlot>();
        protected float timeSinceLastClick = 0f;
        protected float clickCooldown = 0.3f;

        public virtual bool IsBeingUsed => owner != null;

        public Menu()
        {
            float deviceHeight = D3DDevice.Instance.Height;
            float deviceWidth = D3DDevice.Instance.Width;

            position = new TGCVector2(deviceWidth * 0.3f, deviceHeight * 0.1f);
        }

        public abstract void Update(float elapsedTime);

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

        protected abstract void UpdateSlotDisplay();

        protected void CheckClicks()
        {
            TgcD3dInput input = owner.GameInstance.Input;
            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT) && timeSinceLastClick >= clickCooldown)
            {
                TGCVector2 clickPosition = new TGCVector2(input.Xpos, input.Ypos);

                MenuSlot selectedSlot = slots.Find(slot => slot.IsSelected(clickPosition));

                if (selectedSlot != null)
                    selectedSlot.OnClick(owner);
                else
                    Close();

                timeSinceLastClick = 0;
            }
        }

        public virtual void Open(Player user)
        {
            if (IsBeingUsed)
                Close();

            owner = user;
            owner.GameInstance.MouseEnable();
        }

        public virtual void Close()
        {
            if (!IsBeingUsed)
                throw new Exception("The menu doesn´t have an owner.");

            owner.GameInstance.MouseDisable();
            owner = null;
        }
    }
}
