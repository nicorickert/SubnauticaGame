using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    public class Player : GameObject
    {
        #region UTILS
        private readonly float timePerHitTick = 1f;
        private float timeSinceLastTick = 0f;
        private readonly float interactionCooldown = 1f;
        private float timeSinceLastInteraction = 0f;
        private bool godMode = true;
        private TGCMatrix nextTransform = TGCMatrix.Identity;
        private TGCVector3 nextPosition;
        #endregion

        #region STATS
        public int Health { get; private set; } = 100;
        public int Oxygen { get; private set; } = 100;
        public List<Item> Inventory { get; private set; } = new List<Item>();

        private readonly float movementSpeed = 800f;
        private readonly int maxHealth = 100;
        private readonly int oxygenCapacity = 100;
        private readonly int interactionRange = 700;

        private bool IsAlive { get { return Health > 0; } }
        private bool IsOutOfOxygen { get { return Oxygen == 0; } }
        #endregion

        public Player(Subnautica gameInstance, string name, List<TgcMesh> meshes) : base(gameInstance, name, meshes)
        {
            Position = new TGCVector3(0, 100, 2000);
        }

        #region TGC

        public override void Update()
        {
            nextTransform = TGCMatrix.Identity;

            if (IsAlive)
            {
                FixRotation(); // Es importante que esto este antes que ManageMovement()
                ManageMovement();
                SimulateAndSetTransformation(nextPosition, nextTransform);
                CheckInteraction();
                UpdateVitals();
            }
        }

        #endregion

        #region PRIVATE_METHODS

        private void ManageMovement()
        {
            TgcD3dInput input = GameInstance.Input;
            TGCVector3 movementDirection = TGCVector3.Empty;
            TGCVector3 rotationVector = TGCVector3.Empty;

            if (input.keyDown(Key.W))  // Adelante
            {
                movementDirection += LookDirection;
            }
            else if (input.keyDown(Key.S))  // Atras
            {
                movementDirection -= LookDirection;
            }

            if (input.keyDown(Key.A))  // Izquierda
            {
                TGCQuaternion quat = TGCQuaternion.RotationAxis(RelativeUpDirection, FastMath.PI_HALF);
                TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(quat);
                movementDirection += MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }
            else if (input.keyDown(Key.D))  // Derecha
            {
                TGCQuaternion quat = TGCQuaternion.RotationAxis(RelativeUpDirection, FastMath.PI_HALF * 3);
                TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(quat);
                movementDirection += MathExtended.TransformVector3(rotationMatrix, LookDirection);
            }

            if (input.keyDown(Key.Space))    // De acá para abajo son solo de prueba para poder moverme en la escena libremente
            {
                movementDirection += new TGCVector3(0, 1, 0);
            }
            else if (input.keyDown(Key.X))
            {
                movementDirection += new TGCVector3(0, -1, 0);
            }

            /* Simulate transform */
            TGCVector3 totalTranslation = TGCVector3.Normalize(movementDirection) * movementSpeed * GameInstance.ElapsedTime;
            TGCMatrix translationMatrix = TGCMatrix.Translation(Position);

            nextPosition = Position + totalTranslation;
            nextTransform *= translationMatrix;
        }

        private void FixRotation()
        {
            TGCVector3 rotationAxis = TGCVector3.Cross(InitialLookDirection, LookDirection);  // Ojo el orden - no es conmutativo
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(rotationAxis, MathExtended.AngleBetween(InitialLookDirection, LookDirection));
            TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(rotation);
            nextTransform *= rotationMatrix;  // TODO Ver cuando estén las colisiones si hay que hacer la rotacion respecto de la cabeza o desde los pies (actualmente desde los pies)
        }

        private bool IsSubmerged() => Position.Y < GameInstance.WaterY;

        private void UpdateVitals()
        {
            if (godMode) // Para no morirme y poder explorar tranquilo
                return;

            timeSinceLastTick += GameInstance.ElapsedTime;

            if(timeSinceLastTick >= timePerHitTick)
            {
                if (IsSubmerged())
                {
                    if (IsOutOfOxygen)
                    {
                        AddHealth(-10);
                    }
                    else
                    {
                        AddOxygen(-10);
                    }
                }
                else
                {
                    Oxygen = oxygenCapacity;
                }

                timeSinceLastTick = 0;
            }

        }

        private void CheckInteraction()
        {
            timeSinceLastInteraction += GameInstance.ElapsedTime;

            if (timeSinceLastInteraction >= interactionCooldown && GameInstance.Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT)) // No funciona el buttonPressed
            {
                TgcPickingRay pickingRay = new TgcPickingRay(GameInstance.Input);
                GameObject selectedObject = null;

                pickingRay.updateRay();
                selectedObject = ReachableObjects().Find(obj => obj.CheckRayCollision(pickingRay));

                if (selectedObject != null)
                {
                    selectedObject.Interact(this);
                    timeSinceLastInteraction = 0;
                }
            }
        }

        protected List<GameObject> ReachableObjects() => ObjectsWithinRange(interactionRange);

        #endregion

        #region INTERFACE

        public void AddHealth(int quantity)
        {
            Health = FastMath.Clamp(Health + quantity, 0, maxHealth);
        }

        public void AddOxygen(int quantity)
        {
            Oxygen = FastMath.Clamp(Oxygen + quantity, 0, oxygenCapacity);
        }

        public void CollectItem(Item item)
        {
            Inventory.Add(item);
        }

        #endregion
    }
}
