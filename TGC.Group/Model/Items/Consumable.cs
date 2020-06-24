using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Sound;

namespace TGC.Group.Model.Items
{
    public class Consumable : Item
    {
        public override string ItemTypeDescription => "consumible";

        public Consumable(EItemID id, string name, List<IItemEffect> onUseEffect)
            : base(id, name, onUseEffect) { }


        public override void Use(Player user)
        {
            base.Use(user);
            user.GameInstance.EatingSound.play();
            user.Inventory.Remove(this);
        }
    }
}
