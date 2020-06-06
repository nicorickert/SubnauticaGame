using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Items.Effects;
using TGC.Group.Model.Utils.Sprites;

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

        public Dictionary<EItemID, CustomSprite> ItemSprites = new Dictionary<EItemID, CustomSprite>();

        private ItemDatabase()
        {
            ItemSprites[EItemID.CORAL_ARMOR] = new CustomSprite(mediaDir + "Sprites\\coralArmorSprite.png");
            ItemSprites[EItemID.CORAL_PIECE] = new CustomSprite(mediaDir + "Sprites\\coralPieceSprite.png");
            ItemSprites[EItemID.FISH_SCALE] = new CustomSprite(mediaDir + "Sprites\\fishScaleSprite.png");
            ItemSprites[EItemID.FISH_SOUP] = new CustomSprite(mediaDir + "Sprites\\fishSoupSprite.png");
            ItemSprites[EItemID.METAL_SCRAP] = new CustomSprite(mediaDir + "Sprites\\metalScrapSprite.png");
            ItemSprites[EItemID.OXYGEN_TANK] = new CustomSprite(mediaDir + "Sprites\\oxygenTankSprite.png");
            ItemSprites[EItemID.RAW_FISH] = new CustomSprite(mediaDir + "Sprites\\fishSprite.png");
            ItemSprites[EItemID.RAW_SHARK] = new CustomSprite(mediaDir + "Sprites\\sharkSprite.png");
            ItemSprites[EItemID.SHARK_TOOTH] = new CustomSprite(mediaDir + "Sprites\\sharkToothSprite.png");
            ItemSprites[EItemID.SHARK_TOOTH_KNIFE] = new CustomSprite(mediaDir + "Sprites\\sharkToothKnifeSprite.png");
        }

        public Item Generate(EItemID itemID)
        {
            Item generatedItem = null;
            List<IItemEffect> effects = new List<IItemEffect>();

            switch (itemID)
            {
                case (EItemID.RAW_FISH):
                    effects.Add(new Heal(20));
                    generatedItem = new Consumable(itemID, "Carne de pescado", effects);
                    break;

                case (EItemID.RAW_SHARK):
                    effects.Add(new Heal(80));
                    generatedItem = new Consumable(itemID, "Carne de tiburon", effects);
                    break;

                case (EItemID.FISH_SOUP):
                    effects.Add(new Heal(100));
                    generatedItem = new Consumable(itemID, "Sopa de pescados", effects);
                    break;

                case (EItemID.CORAL_PIECE):
                    generatedItem = new Item(itemID, "Pedazo de coral", effects);
                    break;

                case (EItemID.CORAL_ARMOR):
                    effects.Add(new MaxHealthIncrease(50));
                    generatedItem = new Equipable(itemID, "Armadura de coral", effects, EBodyPart.BODY);
                    break;

                case (EItemID.FISH_SCALE):
                    generatedItem = new Item(itemID, "Escama de pescado", effects);
                    break;

                case (EItemID.SHARK_TOOTH):
                    generatedItem = new Item(itemID, "Diente de tiburon", effects);
                    break;

                case (EItemID.METAL_SCRAP):
                    generatedItem = new Item(itemID, "Reciduo metalico", effects);
                    break;

                case (EItemID.SHARK_TOOTH_KNIFE):
                    effects.Add(new IncreaseAttackDamage(40));
                    generatedItem = new Equipable(itemID, "Cuchillo de diente de tiburon", effects, EBodyPart.WEAPON);
                    break;

                case (EItemID.OXYGEN_TANK):
                    effects.Add(new IncreaseOxyenCapacity(100));
                    generatedItem = new Equipable(itemID, "Tanque de oxigeno", effects, EBodyPart.BACK);
                    break;
            }

            // Si no matchea con ninguno (deberia ser imposible) explota al tratar de usarlo xq es null
            return generatedItem;
        }

        public void Dispose()
        {
            foreach (var sprite in ItemSprites.Values)
            {
                sprite.Dispose();
            }
        }
    }
}
