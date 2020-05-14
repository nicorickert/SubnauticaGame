using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public abstract class Item
    {
        public string Sprite { get; protected set; } // El tipo se cambiará por lo que sea que use TGC
        public string Name { get; protected set; } = "Unnamed";

        public Item(string name, string spritePath)
        {
            Name = name;
            Sprite = spritePath; // new Image(spritePath); Inicializo el sprite correspondiente
        }

        public void Render(Point position)
        {
            // Renderizar el sprite en la posicion dada
        }

        #region INTERFACE
        public abstract void Use(Player user);
        #endregion
    }
}
