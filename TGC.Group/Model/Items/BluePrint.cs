
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public struct ItemAmount
    {
        public readonly EItemID itemId;
        public readonly int amount;

        public ItemAmount(EItemID itemId, int amount)
        {
            this.itemId = itemId;
            this.amount = amount;
        }
    }

    public class BluePrint
    {
        private readonly List<ItemAmount> requirements;
        private string _rawDescription;

        public EItemID ProductId { get; private set; }
        public string Description
        {
            get
            {
                string finalDescription = _rawDescription + "(";

                foreach (var req in requirements)
                {
                    finalDescription += req.amount + " " + req.itemId.ToString() + ", ";
                }

                finalDescription += ")";
                return finalDescription;
            }
        }

        public BluePrint(List<ItemAmount> requirements, EItemID productId, string description)
        {
            this.requirements = requirements;
            this.ProductId = productId;
            _rawDescription = description;
        }


        public Item Craft(Player crafter)
        {
            Item craftedItem = null;

            if (CanCraft(crafter))
            {
                craftedItem = ItemDatabase.Instance.Generate(ProductId);
                RemoveRequirementsFrom(crafter);
            }

            return craftedItem;
        }

        private bool CanCraft(Player crafter) => requirements.All(req => crafter.Inventory.Amount(req.itemId) >= req.amount);

        private void RemoveRequirementsFrom(Player crafter)
        {
            foreach (var req in requirements)
            {
                for (int i = 0; i < req.amount; i++)
                {
                    Item itemToRemove = crafter.Inventory.Find(item => item.ID == req.itemId);
                    crafter.Inventory.Remove(itemToRemove);
                }
            }
        }
    }
}
