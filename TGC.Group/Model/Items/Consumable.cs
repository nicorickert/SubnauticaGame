using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public class Consumable : Item
    {
        public override string ItemTypeDescription => "consumible";

        public Consumable(EItemID id, string name, string spritePath, List<IItemEffect> onUseEffect)
            : base(id, name, spritePath, onUseEffect) { }


        public override void Use(Player user)
        {
            base.Use(user);
            user.Inventory.Remove(this);
        }
    }
}
