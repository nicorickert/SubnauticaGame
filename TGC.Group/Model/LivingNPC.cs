using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    public abstract class LivingNPC : GameObject
    {
        protected Dictionary<Item, int> dropsProbability = new Dictionary<Item, int>(); // en %
        protected readonly int maxHealth = 10;
        protected int health = 10;

        protected bool IsAlive { get { return health != 0; } }

        public LivingNPC(Subnautica gameInstance, string name, List<TgcMesh> meshes, int maxHealth) : base(gameInstance, name, meshes)
        {
            this.maxHealth = maxHealth;
            health = maxHealth;
        }

        #region GAME_OBJECT
        public override void Interact(Player interactor)
        {
            AddHealth(-1 * interactor.AttackDamage);

            if (!IsAlive)
            {
                foreach (Item item in dropsProbability.Keys)
                {
                    if (dropsProbability[item] > MathExtended.GetRandomNumberBetween(0, 100))
                        interactor.CollectItem(item);
                }
                    
                Destroy();
            }
        }
        #endregion

        protected void AddHealth(int quantity)
        {
            health = FastMath.Clamp(health + quantity, 0, maxHealth);
        }
    }
}
