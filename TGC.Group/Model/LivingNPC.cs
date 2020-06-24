using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    public abstract class LivingNPC : GameObject
    {
        protected Dictionary<Item, int> dropsProbability = new Dictionary<Item, int>(); // en %
        protected readonly int maxHealth = 10;
        protected int health = 10;

        private List<Tgc3dSound> onHitSounds = new List<Tgc3dSound>();

        protected bool IsAlive { get { return health != 0; } }

        public LivingNPC(Subnautica gameInstance, string name, List<TgcMesh> meshes, int maxHealth) 
            : base(gameInstance, name, meshes)
        {
            this.maxHealth = maxHealth;
            health = maxHealth;

            Tgc3dSound hit1 = new Tgc3dSound(GameInstance.MediaDir + "//Sounds//Golpe1.wav", Position, GameInstance.DirectSound.DsDevice);
            Tgc3dSound hit2 = new Tgc3dSound(GameInstance.MediaDir + "//Sounds//Golpe2.wav", Position, GameInstance.DirectSound.DsDevice);
            Tgc3dSound hit3 = new Tgc3dSound(GameInstance.MediaDir + "//Sounds//Golpe3.wav", Position, GameInstance.DirectSound.DsDevice);

            onHitSounds.Add(hit1);
            onHitSounds.Add(hit2);
            onHitSounds.Add(hit3);

            foreach (var sound in onHitSounds)
                sound.MinDistance = 50f;
        }

        #region GAME_OBJECT
        public override void Interact(Player interactor)
        {
            AddHealth(-1 * interactor.AttackDamage);
            onHitSounds[MathExtended.GetRandomNumberBetween(0, onHitSounds.Count)].play();

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
