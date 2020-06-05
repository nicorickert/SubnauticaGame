using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items.Effects
{
    class IncreaseOxyenCapacity : IItemEffect
    {
        private readonly int capacityIncrease;

        public IncreaseOxyenCapacity(int capacityIncrease)
        {
            this.capacityIncrease = capacityIncrease;
        }

        public void Affect(Player user)
        {
            user.IncreaseOxygenCapacity(capacityIncrease);
        }

        public string Description() => "incrementa la capacidad maxima de oxigeno en " + capacityIncrease + " puntos";

        public void Disaffect(Player user)
        {
            user.IncreaseOxygenCapacity(-capacityIncrease);
        }
    }
}
