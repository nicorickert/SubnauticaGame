using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items.Effects
{
    class MaxHealthIncrease : IItemEffect
    {
        private int maxHealthIncrease;

        public MaxHealthIncrease(int maxHealthIncrease)
        {
            this.maxHealthIncrease = maxHealthIncrease;
        }

        public void Affect(Player user)
        {
            user.IncreaseMaxHealth(maxHealthIncrease);
        }

        public string Description() => "aummenta la vida maxima del usuario en " + maxHealthIncrease + " puntos";

        public void Disaffect(Player user)
        {
            user.IncreaseMaxHealth(-maxHealthIncrease);
        }
    }
}
