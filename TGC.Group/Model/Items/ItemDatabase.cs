using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Items.Effects;

namespace TGC.Group.Model.Items
{
    public enum EItemID
    {
        RAW_FISH,
        RAW_SHARK,
        FISH_SOUP,
        CORAL_PIECE
    }


    public static class ItemDatabase
    {
        public static Item Generate(EItemID itemID)
        {
            Item generatedItem = null;
            List<IItemEffect> effects = new List<IItemEffect>();

            switch (itemID)
            {
                case (EItemID.RAW_FISH):
                    effects.Add(new Heal(20));
                    generatedItem = new Consumable(itemID, "Raw fish", "un path", effects);
                    break;

                case (EItemID.RAW_SHARK):
                    effects.Add(new Heal(80));
                    generatedItem = new Consumable(itemID, "Raw shark", "un path", effects);
                    break;

                case (EItemID.FISH_SOUP):
                    effects.Add(new Heal(100));
                    generatedItem = new Consumable(itemID, "Fish soup", "un path", effects);
                    break;

                case (EItemID.CORAL_PIECE):
                    generatedItem = new Item(itemID, "Coral piece", "un path", effects);
                    break;
            }

            // Si no matchea con ninguno (deberia ser imposible) explota al tratar de usarlo xq es null
            return generatedItem;
        }
    }
}
