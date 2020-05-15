using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public interface IItemEffect
    {
        void Affect(Player user);
        void Disaffect(Player user);
    }
}
