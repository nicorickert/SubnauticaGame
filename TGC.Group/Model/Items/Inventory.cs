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
        public int Size { get { return Items.Count; } }
        public bool IsEmpty { get { return Items.Count == 0; } }

        public Inventory() { }


        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public Item GetItem(int index) => Items[index];

        public void Remove(Item item)
        {
            Items.Remove(item);
        }

        public int Amount(EItemID itemId) => Items.Count(item => item.ID == itemId);

        public Item Find(Predicate<Item> match) => Items.Find(match);
    }
}
