using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    public class Ship : GameObject
    {
        private TgcText2D craftingMenu = new TgcText2D();
        private bool usingCraftingMenu = false;
        private Player crafter = null;
        private readonly float craftingCooldown = 1f;
        private float timeSinceLastSelection = 0f;

        public Ship(Subnautica gameInstance, string name, List<TgcMesh> meshes) : base(gameInstance, name, meshes)
        {
            Position = new TGCVector3(3500, 60, 0);   // seteo la posicion del barco
            scale *= 4;
            rotation = new TGCVector3(0, FastMath.PI_HALF, 0);

            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix scaling = TGCMatrix.Scaling(scale);
            TGCMatrix rot = TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

            Transform = rot * scaling * translation;

            int deviceWidth = D3DDevice.Instance.Width;
            int deviceHeight = D3DDevice.Instance.Height;

            craftingMenu.Color = Color.DarkOrange;
            craftingMenu.Position = new Point((int)FastMath.Floor(0.3f * deviceWidth), (int)FastMath.Floor(0.2f * deviceHeight));
            craftingMenu.Size = new Size(800, 200);
            craftingMenu.changeFont(new Font("TimesNewRoman", 20, FontStyle.Regular));
            craftingMenu.Align = TgcText2D.TextAlign.LEFT;
        }

        #region TGC

        public override void Update()
        {
            if(crafter != null)
            {
                UpdateCraftingMenu();
                ChooseBluePrint();

                if (!GameInstance.Player.CanReach(this))
                    CloseCraftingMenu();
            }
        }

        public override void Render()
        {
            base.Render();

            if (usingCraftingMenu)
                craftingMenu.render();
        }

        #endregion

        public override void Interact(Player interactor)
        {
            OpenCraftingMenuFor(interactor);
        }

        private void OpenCraftingMenuFor(Player crafter) { usingCraftingMenu = true; this.crafter = crafter; }
        private void CloseCraftingMenu() { usingCraftingMenu = false; crafter = null; }

        private void UpdateCraftingMenu()
        {
            craftingMenu.Text = "Blue prints: \n";

            int index = 1;
            foreach (var bp in crafter.AvailableBluePrints)
            {
                craftingMenu.Text += "F" + index + ") " + bp.Description + "\n";
                index++;
            }
        }

        private void ChooseBluePrint()
        {
            timeSinceLastSelection += GameInstance.ElapsedTime;

            if (timeSinceLastSelection < craftingCooldown)
                return;


            int selection;

            if (GameInstance.Input.keyDown(Key.F1))
                selection = 1;
            else if (GameInstance.Input.keyDown(Key.F2))
                selection = 2;
            else if (GameInstance.Input.keyDown(Key.F3))
                selection = 3;
            else if (GameInstance.Input.keyDown(Key.F4))
                selection = 4;
            else if (GameInstance.Input.keyDown(Key.F5))
                selection = 5;
            else
                return;

            timeSinceLastSelection = 0f;

            if (selection > crafter.AvailableBluePrints.Count)
                return; // No hago nada, no existe tal blueprint

            int bpIndex = selection - 1;
            Item craftedItem = crafter.AvailableBluePrints[bpIndex].Craft(crafter);

            if (craftedItem != null)
                crafter.CollectItem(craftedItem);
        }
    }
}
