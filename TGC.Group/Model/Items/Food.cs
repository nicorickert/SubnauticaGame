using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    class Food : Item
    {
        private readonly int healingFactor = 10;


        public Food(string name, string spritePath, int healingFactor) : base(name, spritePath)
        {
            this.healingFactor = healingFactor;
        }


        public override void Use(Player user)
        {
            user.AddHealth(healingFactor);
            user.Inventory.Remove(this);
        }
    }
}
