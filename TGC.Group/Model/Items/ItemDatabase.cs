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

    public struct ItemInfo
    {
        public string Name { get; }
        public string SpritePath { get; }


        public ItemInfo(string name, string spritePath)
        {
            Name = name;
            SpritePath = spritePath;
        }
    }


    public class ItemDatabase
    {
        #region SINGLETON
        private static ItemDatabase _instance = null;
        public static ItemDatabase Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ItemDatabase();

                return _instance;
            }
        }
        #endregion

        private string mediaDir = Game.Default.MediaDirectory;

        public Dictionary<EItemID, ItemInfo> ItemsInfo { get; } = new Dictionary<EItemID, ItemInfo>();

        private ItemDatabase()
        {
            ItemsInfo[EItemID.RAW_FISH] = new ItemInfo("Raw fish", mediaDir + "cajaMadera4.jpg");
            ItemsInfo[EItemID.RAW_SHARK] = new ItemInfo("Raw shark", mediaDir + "cajaMadera4.jpg");
            ItemsInfo[EItemID.FISH_SOUP] = new ItemInfo("Fish soup", mediaDir + "cajaMadera4.jpg");
            ItemsInfo[EItemID.CORAL_PIECE] = new ItemInfo("Coral piece", mediaDir + "cajaMadera4.jpg");
        }

        public Item Generate(EItemID itemID)
        {
            Item generatedItem = null;
            List<IItemEffect> effects = new List<IItemEffect>();

            switch (itemID)
            {
                case (EItemID.RAW_FISH):
                    effects.Add(new Heal(20));
                    generatedItem = new Consumable(itemID, ItemsInfo[itemID].Name, ItemsInfo[itemID].SpritePath, effects);
                    break;

                case (EItemID.RAW_SHARK):
                    effects.Add(new Heal(80));
                    generatedItem = new Consumable(itemID, ItemsInfo[itemID].Name, ItemsInfo[itemID].SpritePath, effects);
                    break;

                case (EItemID.FISH_SOUP):
                    effects.Add(new Heal(100));
                    generatedItem = new Consumable(itemID, ItemsInfo[itemID].Name, ItemsInfo[itemID].SpritePath, effects);
                    break;

                case (EItemID.CORAL_PIECE):
                    generatedItem = new Item(itemID, ItemsInfo[itemID].Name, ItemsInfo[itemID].SpritePath, effects);
                    break;
            }

            // Si no matchea con ninguno (deberia ser imposible) explota al tratar de usarlo xq es null
            return generatedItem;
        }


    }
}
