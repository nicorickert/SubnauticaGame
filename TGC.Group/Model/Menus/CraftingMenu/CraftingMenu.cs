using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Sound;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus.CraftingMenu
{
    class CraftingMenu : Menu
    {
        private bool craftingCooldownEnabled = false;
        private CustomBitmap bitmapSlotBackground;
        private CustomBitmap bitmapSlotProduct;
        private TgcStaticSound openSound = new TgcStaticSound();

        public Player Owner => owner;

        public CraftingMenu() 
            : base() {
            // Imagenes para los slots
            bitmapSlotBackground = new CustomBitmap(Game.Default.MediaDirectory + "craftingSlotBackground.jpg", D3DDevice.Instance.Device);
            bitmapSlotProduct = new CustomBitmap(Game.Default.MediaDirectory + "cajaMadera4.jpg", D3DDevice.Instance.Device);
        }

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

            openSound.loadSound(crafter.GameInstance.MediaDir + "//Sounds//AbrirMenuCrafteo.wav", crafter.GameInstance.DirectSound.DsDevice);
            openSound.play();

            UpdateSlotDisplay();
            timeSinceLastClick = 0f;
            craftingCooldownEnabled = true;
        }

        public override void Close()
        {
            base.Close();

            craftingCooldownEnabled = false;
        }

        public override void UpdateSlotDisplay()
        {
            slots.Clear();

            TGCVector2 nextPosition = position;
            foreach (var bp in owner.AvailableBluePrints)
            {
                CraftingSlot craftingSlot = new CraftingSlot(nextPosition, bp, this, bitmapSlotBackground, bitmapSlotProduct);
                slots.Add(craftingSlot);
                nextPosition.Y += craftingSlot.Size.Height + 30;
            }
        }
    }
}
