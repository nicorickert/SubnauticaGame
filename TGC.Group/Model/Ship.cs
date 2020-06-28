using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Items;
using TGC.Group.Model.Materials;
using TGC.Group.Model.Menus.CraftingMenu;

namespace TGC.Group.Model
{
    public class Ship : GameObject
    {
        public override Material MaterialInfo => Material.Metalic;

        private CraftingMenu craftingMenu = new CraftingMenu();


        public Ship(Subnautica gameInstance, string name, List<TgcMesh> meshes) 
            : base(gameInstance, name, meshes)
        {
            Position = new TGCVector3(400, 60, -900);   // seteo la posicion del barco
            scale *= 4;
            rotation = new TGCVector3(0, FastMath.PI_HALF, 0);

            TGCMatrix translation = TGCMatrix.Translation(Position);
            TGCMatrix scaling = TGCMatrix.Scaling(scale);
            TGCMatrix rot = TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z);

            Transform = rot * scaling * translation;
        }

        #region TGC

        public override void Update()
        {
            base.Update();

            craftingMenu.Update(GameInstance.ElapsedTime);

            if (craftingMenu.IsBeingUsed && !craftingMenu.Owner.CanReach(this))
            {
                craftingMenu.Close();
            }
        }

        public override void Render()
        {
            base.Render();

            if (craftingMenu.IsBeingUsed)
                craftingMenu.Render();
        }

        public override void Dispose()
        {
            base.Dispose();

            craftingMenu.Dispose();
        }

        #endregion

        public override void Interact(Player interactor)
        {
            if(!craftingMenu.IsBeingUsed)
                craftingMenu.Open(interactor);
        }

        public void CloseCraftingMenu()
        {
            if (craftingMenu.IsBeingUsed)
                craftingMenu.Close();
        }
    }
}
