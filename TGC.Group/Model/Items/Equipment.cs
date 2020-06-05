using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public class Equipment
    {
        private Dictionary<EBodyPart, Equipable> slots = new Dictionary<EBodyPart, Equipable>();
        private Player owner;

        public Equipment(Player owner)
        {
            this.owner = owner;

            slots[EBodyPart.LEGS] = null;
            slots[EBodyPart.BODY] = null;
            slots[EBodyPart.HEAD] = null;
            slots[EBodyPart.BACK] = null;
            slots[EBodyPart.WEAPON] = null;
        }

        public void UnEquip(EBodyPart bodyPart)
        {
            if (slots[bodyPart] == null)
                return;

            Equipable equipable = slots[bodyPart];

            slots[bodyPart] = null;

            equipable.UnUse(owner);
            owner.CollectItem(equipable);
        }

        public void Equip(Equipable item)
        {
            EBodyPart bodyPart = item.BodyPart;

            if (slots[bodyPart] != null)
                UnEquip(bodyPart);

            slots[bodyPart] = item;
            owner.Inventory.Remove(item);
        }
    }
}
