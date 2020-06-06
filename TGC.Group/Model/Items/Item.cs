using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public class Item
    {
        public virtual string ItemTypeDescription { get; } = "item";

        public EItemID ID { get; private set; }
        public string Name { get; protected set; }
        public string Description
        {
            get
            {
                float effectsNumber = onUseEffects.Count;

                if (effectsNumber == 0)
                    return "Un recurso.";

                string description = "Un " + ItemTypeDescription + " que ";

                if (effectsNumber == 1)
                    description += onUseEffects[0].Description() + ".";
                else
                {
                    int index = 0;
                    foreach (var effect in onUseEffects)
                    {
                        if (index == effectsNumber - 1) // es el ultimo
                            description += " y " + effect.Description() + ".";
                        else if (index == 0)
                            description += effect.Description();
                        else
                            description += ", " + effect.Description();

                        index++;
                    }
                }

                return description;
            }
        }

        private List<IItemEffect> onUseEffects;

        public Item(EItemID id, string name, List<IItemEffect> onUseEffects)
        {
            ID = id;
            Name = name;
            this.onUseEffects = onUseEffects;
        }

        public void Render(Point position)
        {
            // Renderizar el sprite en la posicion dada
        }

        #region INTERFACE
        public virtual void Use(Player user) // Template method
        {
            foreach (var effect in onUseEffects)
                effect.Affect(user);
        }

        public virtual void UnUse(Player user)
        {
            foreach (var effect in onUseEffects)
                effect.Disaffect(user);
        }
        #endregion
    }
}
