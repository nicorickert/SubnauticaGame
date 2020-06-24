
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Sound;

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
        private TgcStaticSound craftingSound = new TgcStaticSound();
        private readonly List<ItemAmount> requirements;

        public EItemID ProductId { get; private set; }
        public string Description
        {
            get
            {
                string requirementsDescription = "Con ";
                ItemDatabase db = ItemDatabase.Instance;

                foreach (var req in requirements)
                {
                    requirementsDescription += req.amount + " " + db.Generate(req.itemId).Name + ", ";
                }

                Item productSample = db.Generate(ProductId);
                string productDescription = "construir un " + productSample.Name + ". " + productSample.Description;

                return requirementsDescription + productDescription;
            }
        }

        public BluePrint(List<ItemAmount> requirements, EItemID productId)
        {
            this.requirements = requirements;
            ProductId = productId;
        }


        public void Craft(Player crafter)
        {
            if (CanCraft(crafter))
            {
                Item craftedItem = ItemDatabase.Instance.Generate(ProductId);
                RemoveRequirementsFrom(crafter);
                crafter.CollectItem(craftedItem);

                var soundNumber = MathExtended.GetRandomNumberBetween(1, 3);
                craftingSound.loadSound(crafter.GameInstance.MediaDir + "//Sounds//Construir"+soundNumber+".wav", crafter.GameInstance.DirectSound.DsDevice);
                craftingSound.play();
            }
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
