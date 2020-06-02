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
    }
}
