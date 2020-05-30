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
        public EItemID ID { get; private set; }
        public string Sprite { get; protected set; } // El tipo se cambiará por lo que sea que use TGC
        public string Name { get; protected set; } = "Unnamed";

        private List<IItemEffect> onUseEffects;

        public Item(EItemID id, string name, string spritePath, List<IItemEffect> onUseEffects)
        {
            ID = id;
            Name = name;
            Sprite = spritePath; // new Image(spritePath); Inicializo el sprite correspondiente
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
        #endregion
    }
}
