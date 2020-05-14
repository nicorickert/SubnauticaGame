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
    class Shark : Roamer
    {
        private TGCVector3 lastRoamingLookDirection;
        private readonly float attackCooldown = 2f;
        private float timeSinceLastAttack = 0;

        private readonly int maxHealth = 100;
        private int health = 100;
        private readonly float chasingSpeed = 400f;
        private readonly int attackDamage = 40;
        private readonly float attackRange = 600f;

        private bool IsChasing { get { return NearObjects().Contains(GameInstance.Player) && GameInstance.Player.Position.Y < GameInstance.WaterY; } }
        private bool IsAlive { get { return health != 0; } }

        public Shark(Subnautica gameInstace, string name, List<TgcMesh> meshes, TGCVector3 spawnLocation)
            : base(gameInstace, name, meshes, 200f)
        {
            InitialLookDirection = new TGCVector3(-1, 0, 0);
            LookDirection = InitialLookDirection;
            lastRoamingLookDirection = InitialLookDirection;

            Position = spawnLocation;
            Transform = TGCMatrix.Translation(Position);
        }

        #region TGC
        public override void Update()
        {
            if (IsChasing)
            {
                Chase();
                HitPlayer();
            }
            else
            {
                LookDirection = lastRoamingLookDirection;
                Roam();
                lastRoamingLookDirection = LookDirection;
            }
        }
        #endregion

        private void Chase()
        {
            LookDirection = TGCVector3.Normalize(GameInstance.Player.Position - Position);

            TGCVector3 rotationAxis = TGCVector3.Cross(InitialLookDirection, LookDirection);  // Ojo el orden - no es conmutativo
            TGCQuaternion rotationQuat = TGCQuaternion.RotationAxis(rotationAxis, MathExtended.AngleBetween(InitialLookDirection, LookDirection));

            TGCVector3 nextPosition = Position + LookDirection * chasingSpeed * GameInstance.ElapsedTime;

            TGCMatrix translationMatrix = TGCMatrix.Translation(nextPosition);
            TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(rotationQuat);

            TGCMatrix nextTransform = rotationMatrix * translationMatrix;

            SimulateAndSetTransformation(nextPosition, nextTransform);
        }

        private void AddHealth(int quantity)
        {
            health = FastMath.Clamp(health + quantity, 0, maxHealth);
        }

        public override void Interact(Player interactor)
        {
            AddHealth(-1 * interactor.AttackDamage);

            if (!IsAlive)
            {
                interactor.CollectItem(new Food("raw_shark_meat", "algun path", 80));
                Destroy();
            }
        }

        private void HitPlayer()
        {
            timeSinceLastAttack += GameInstance.ElapsedTime;

            float playerDistance = new TGCVector3(GameInstance.Player.Position - Position).Length();

            if (playerDistance <= attackRange && timeSinceLastAttack >= attackCooldown)
            {
                GameInstance.Player.AddHealth(-1 * attackDamage);
                Console.WriteLine("Player hit by: " + attackDamage);
                timeSinceLastAttack = 0f;
            }
        }
    }
}
