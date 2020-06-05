using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items.Effects
{
    class IncreaseAttackDamage : IItemEffect
    {
        private readonly int damageIncrease;

        public IncreaseAttackDamage(int damageIncrease)
        {
            this.damageIncrease = damageIncrease;
        }

        public void Affect(Player user)
        {
            user.IncreaseAttackDamage(damageIncrease);
        }

        public string Description() => "aumenta el daño de ataque del usuario en " + damageIncrease + " puntos";

        public void Disaffect(Player user)
        {
            user.IncreaseAttackDamage(-damageIncrease);
        }
    }
}
