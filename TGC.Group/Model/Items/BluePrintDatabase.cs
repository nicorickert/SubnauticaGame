using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Items
{
    public static class BluePrintDatabase
    {
        public static BluePrint FishSoup
        {
            get
            {
                ItemAmount req1 = new ItemAmount(EItemID.RAW_FISH, 1);
                ItemAmount req2 = new ItemAmount(EItemID.RAW_SHARK, 1);
                List<ItemAmount> requirements = new ItemAmount[] { req1, req2 }.ToList();
                return new BluePrint(requirements, EItemID.FISH_SOUP);
            }
        }

        public static BluePrint CoralArmor
        {
            get
            {
                ItemAmount req = new ItemAmount(EItemID.CORAL_PIECE, 5);
                return new BluePrint(new List<ItemAmount>(new ItemAmount[] { req }), EItemID.CORAL_ARMOR);
            }
        }

        public static BluePrint SharkToothKnife
        {
            get
            {
                ItemAmount req1 = new ItemAmount(EItemID.SHARK_TOOTH, 1);
                ItemAmount req2 = new ItemAmount(EItemID.METAL_SCRAP, 1);
                List<ItemAmount> requirements = new ItemAmount[] { req1, req2 }.ToList();
                return new BluePrint(requirements, EItemID.SHARK_TOOTH_KNIFE);
            }
        }

        public static BluePrint OxygenTank
        {
            get
            {
                ItemAmount req1 = new ItemAmount(EItemID.FISH_SCALE, 5);
                ItemAmount req2 = new ItemAmount(EItemID.CORAL_PIECE, 5);
                ItemAmount req3 = new ItemAmount(EItemID.METAL_SCRAP, 3);
                List<ItemAmount> requirements = new ItemAmount[] { req1, req2, req3 }.ToList();
                return new BluePrint(requirements, EItemID.OXYGEN_TANK);
            }
        }
    }
}
