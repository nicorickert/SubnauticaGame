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
        #region SETTINGS
        private TGCVector3 lastRoamingLookDirection;
        private readonly float attackCooldown = 2f;
        private float timeSinceLastAttack = 0;
        #endregion

        private readonly float chasingSpeed = 400f;
        private readonly int attackDamage = 40;
        private readonly float attackRange = 600f;

        private bool IsChasing { get { return NearObjects().Contains(GameInstance.Player) && !GameInstance.Player.IsOutOfTheWater && !GameInstance.Player.IsInSafeZone; } }

        public Shark(Subnautica gameInstace, string name, List<TgcMesh> meshes, TGCVector3 spawnLocation)
            : base(gameInstace, name, meshes, 100, 200f)
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
                TryToHitPlayer();
            }
        }

        public override void Render()
        {
            if (IsChasing)
            {
                Chase();
            }
            else
            {
                LookDirection = lastRoamingLookDirection;
                Roam();
                lastRoamingLookDirection = LookDirection;
            }

            base.Render();
        }
        #endregion

        #region LIVING_NPC
        protected override Item GenerateDrop() => ItemDatabase.Instance.Generate(EItemID.RAW_SHARK);
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

        private void TryToHitPlayer()
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
