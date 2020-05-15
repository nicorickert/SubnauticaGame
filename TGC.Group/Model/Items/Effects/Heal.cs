using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items.Effects
{
    public class Heal : IItemEffect
    {
        int healingFactor = 0;

        public Heal(int healingFactor)
        {
            this.healingFactor = healingFactor;
        }

        public void Affect(Player user)
        {
            user.AddHealth(healingFactor);
        }

        public void Disaffect(Player user)
        {
            user.AddHealth(-healingFactor);
        }
    }
}
