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
        public Dictionary<EItemID, int> AccumulatedItems = new Dictionary<EItemID, int>();

        public Inventory() { }


        public void AddItem(Item item)
        {
            Items.Add(item);

            AccumulatedItems[item.ID] = Amount(item.ID) + 1;
        }

        public Item GetItem(int index) => Items[index];

        public void Remove(Item item)
        {
            Items.Remove(item);

            int newAmount = Amount(item.ID) - 1;
            AccumulatedItems[item.ID] = newAmount;

            if (newAmount == 0)
                AccumulatedItems.Remove(item.ID);
        }

        public int Amount(EItemID itemId) => AccumulatedItems.ContainsKey(itemId) ? AccumulatedItems[itemId] : 0;

        public Item Find(Predicate<Item> match) => Items.Find(match);
    }
}
