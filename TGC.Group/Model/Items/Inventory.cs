using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public class Inventory
    {
        public List<Item> Items { get; } = new List<Item>();  // Despues se puede cambiar para que sea de un tamaño fijo
        public bool IsEmpty { get { return Items.Count == 0; } }

        public Inventory() { }


        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void UseItem(int slot, Player user)
        {
            Items[slot].Use(user);
        }

        public void Remove(Item item)
        {
            Items.Remove(item);
        }

        public int Quantity(EItemID itemId) => Items.Count(item => item.ID == itemId);

        public Item Find(Predicate<Item> match) => Items.Find(match);
    }
}
