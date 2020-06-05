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
        CORAL_PIECE,
        CORAL_ARMOR,
        SHARK_TOOTH,
        METAL_SCRAP,
        FISH_SCALE,
        OXYGEN_TANK,
        SHARK_TOOTH_KNIFE
    }

    //public struct ItemInfo
    //{
    //    public string Name { get; }
    //    public string SpritePath { get; }


    //    public ItemInfo(string name, string spritePath)
    //    {
    //        Name = name;
    //        SpritePath = spritePath;
    //    }
    //}


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

        //public Dictionary<EItemID, ItemInfo> ItemsInfo { get; } = new Dictionary<EItemID, ItemInfo>();

        private ItemDatabase()
        {
            //ItemsInfo[EItemID.RAW_FISH] = new ItemInfo("Raw fish", mediaDir + "cajaMadera4.jpg");
            //ItemsInfo[EItemID.RAW_SHARK] = new ItemInfo("Raw shark", mediaDir + "cajaMadera4.jpg");
            //ItemsInfo[EItemID.FISH_SOUP] = new ItemInfo("Fish soup", mediaDir + "cajaMadera4.jpg");
            //ItemsInfo[EItemID.CORAL_PIECE] = new ItemInfo("Coral piece", mediaDir + "cajaMadera4.jpg");
        }

        public Item Generate(EItemID itemID)
        {
            Item generatedItem = null;
            List<IItemEffect> effects = new List<IItemEffect>();

            switch (itemID)
            {
                case (EItemID.RAW_FISH):
                    effects.Add(new Heal(20));
                    generatedItem = new Consumable(itemID, "Carne de pescado", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.RAW_SHARK):
                    effects.Add(new Heal(80));
                    generatedItem = new Consumable(itemID, "Carne de tiburon", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.FISH_SOUP):
                    effects.Add(new Heal(100));
                    generatedItem = new Consumable(itemID, "Sopa de pescados", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.CORAL_PIECE):
                    generatedItem = new Item(itemID, "Pedazo de coral", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.CORAL_ARMOR):
                    effects.Add(new MaxHealthIncrease(50));
                    generatedItem = new Equipable(itemID, "Armadura de coral", mediaDir + "cajaMadera4.jpg", effects, EBodyPart.BODY);
                    break;

                case (EItemID.FISH_SCALE):
                    generatedItem = new Item(itemID, "Escama de pescado", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.SHARK_TOOTH):
                    generatedItem = new Item(itemID, "Diente de tiburon", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.METAL_SCRAP):
                    generatedItem = new Item(itemID, "Reciduo metalico", mediaDir + "cajaMadera4.jpg", effects);
                    break;

                case (EItemID.SHARK_TOOTH_KNIFE):
                    effects.Add(new IncreaseAttackDamage(40));
                    generatedItem = new Equipable(itemID, "Cuchillo de diente de tiburon", mediaDir + "cajaMadera4.jpg", effects, EBodyPart.WEAPON);
                    break;

                case (EItemID.OXYGEN_TANK):
                    effects.Add(new IncreaseOxyenCapacity(100));
                    generatedItem = new Equipable(itemID, "Tanque de oxigeno", mediaDir + "cajaMadera4.jpg", effects, EBodyPart.BACK);
                    break;
            }

            // Si no matchea con ninguno (deberia ser imposible) explota al tratar de usarlo xq es null
            return generatedItem;
        }


    }
}
