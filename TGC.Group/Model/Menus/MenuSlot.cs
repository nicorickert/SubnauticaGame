using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.Utils.Sprites;

namespace TGC.Group.Model.Menus
{
    public abstract class MenuSlot
    {
        public Size Size { get; protected set; }
        public TGCVector2 Position { get; protected set; }

        public MenuSlot(TGCVector2 position)
        {
            Position = position;
        }

        public abstract void RenderText();
        public abstract void RenderSprites(Drawer2D drawer);
        public abstract void Dispose();
        public abstract void OnClick(Player clicker);

        public bool IsSelected(TGCVector2 clickPosition)
        {
            return clickPosition.X > Position.X && clickPosition.X < Position.X + Size.Width && clickPosition.Y > Position.Y && clickPosition.Y < Position.Y + Size.Height;
        }
    }
}
