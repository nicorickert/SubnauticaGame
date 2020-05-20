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
        private readonly float itemUseCooldown = 1f;
        private float timeSinceLastItemUse = 0f;
        private readonly float timePerHitTick = 1f;
        private float timeSinceLastTick = 0f;
        private readonly float interactionCooldown = 1f;
        private float timeSinceLastInteraction = 0f;
        private readonly float itemSelectionCooldown = 0.2f;
        private float timeSinceLastItemSelected = 0f;
        private bool godMode = true;
        private TGCMatrix nextTransform = TGCMatrix.Identity;
        private TGCVector3 nextPosition;

        public int SelectedItem { get; private set; } = 0;
        public TGCVector3 RelativeEyePosition { get; } = new TGCVector3(0, 120, 30);
        #endregion

        #region STATS
        public int Health { get; private set; } = 100;
        public int Oxygen { get; private set; } = 100;
        public Inventory Inventory { get; private set; } = new Inventory();
        public int AttackDamage { get; private set; } = 10;
        public List<BluePrint> AvailableBluePrints { get; private set; } = new List<BluePrint>();
        public bool IsSubmerged { get { return Position.Y < GameInstance.WaterLevelToWorldHeight(-130f); } }
        public bool IsInTheWater { get { return Position.Y < GameInstance.WaterLevelToWorldHeight(-100f); } }
        public bool CollidingWithFloor { get { return Position.Y <= GameInstance.FloorLevelToWorldHeight(0); } }
        public bool IsOutOfTheWater { get { return Position.Y > GameInstance.WaterLevelToWorldHeight(0); } }
        public bool IsInSafeZone { get => IsWithinRange(interactionRange, GameInstance.Ship); }

        private readonly float movementSpeed = 1000f;
        private readonly int maxHealth = 100;
        private readonly int oxygenCapacity = 100;
        private readonly int interactionRange = 700;

        private bool IsAlive { get { return Health > 0; } }
        private bool IsOutOfOxygen { get { return Oxygen == 0; } }
        #endregion

        public Player(Subnautica gameInstance, string name, List<TgcMesh> meshes) : base(gameInstance, name, meshes)
        {
            Position = new TGCVector3(3300, -50, 700);
            LearnBluePrint(BluePrintDatabase.FishSoup);
        }

        #region TGC

        public override void Update()
        {
            // Para modo god
            if (GameInstance.Input.keyDown(Key.G))
                godMode = true;
            if (GameInstance.Input.keyDown(Key.H))
                godMode = false;

            if (IsAlive)
            {
                CheckInteraction();
                UpdateVitals();
                UpdateSelectedItem();
                CheckItemUse();
            }
        }

        public override void Render()
        {
            nextTransform = TGCMatrix.Identity;

            if (IsAlive)
            {
                FixRotation(); // Es importante que esto este antes que ManageMovement()
                ManageMovement();
                SimulateAndSetTransformation(nextPosition, nextTransform);
            }

            base.Render();
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

            if (input.keyDown(Key.Space))
            {
                movementDirection += new TGCVector3(0, 1, 0);
            }
            else if (input.keyDown(Key.X))
            {
                movementDirection += new TGCVector3(0, -1, 0);
            }

            if ((!IsInTheWater && movementDirection.Y > 0))
                movementDirection.Y = 0;

            if((CollidingWithFloor))
            {
                movementDirection.Y = 0;
                Position = new TGCVector3(Position.X, GameInstance.FloorLevelToWorldHeight(0), Position.Z);
            }


            TGCVector3 totalTranslation = TGCVector3.Normalize(movementDirection) * movementSpeed * GameInstance.ElapsedTime;
            nextPosition = Position + totalTranslation;

            TGCMatrix translationMatrix = TGCMatrix.Translation(Position);
            nextTransform *= translationMatrix;
        }

        private void FixRotation()
        {
            TGCVector3 rotationAxis = TGCVector3.Cross(InitialLookDirection, LookDirection);  // Ojo el orden - no es conmutativo
            float angle = MathExtended.AngleBetween(InitialLookDirection, LookDirection);
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(rotationAxis, angle);
            TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(rotation);
            nextTransform *= rotationMatrix;
        }

        private void UpdateVitals()
        {
            timeSinceLastTick += GameInstance.ElapsedTime;

            if(timeSinceLastTick >= timePerHitTick)
            {
                if (IsSubmerged)
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

        private void CheckItemUse()
        {
            timeSinceLastItemUse += GameInstance.ElapsedTime;

            if (timeSinceLastItemUse >= itemUseCooldown && !Inventory.IsEmpty)
            {
                if (GameInstance.Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
                {
                    Inventory.GetItem(SelectedItem).Use(this);
                    SelectedItem = FastMath.Max(SelectedItem - 1, 0);
                    timeSinceLastItemUse = 0;
                }
            }
        }

        private void UpdateSelectedItem()
        {
            timeSinceLastItemSelected += GameInstance.ElapsedTime;

            if(timeSinceLastItemSelected >= itemSelectionCooldown)
            {
                if (Inventory.IsEmpty)
                    SelectedItem = 0;
                else if (GameInstance.Input.keyDown(Key.UpArrow))
                {
                    SelectedItem--;
                    if (SelectedItem == -1)
                        SelectedItem = Inventory.Size - 1;
                }
                else if (GameInstance.Input.keyDown(Key.DownArrow))
                {
                    SelectedItem++;
                    if (SelectedItem == Inventory.Size)
                        SelectedItem = 0;
                }

                timeSinceLastItemSelected = 0f;
            }
        }

        private List<GameObject> ReachableObjects() => ObjectsWithinRange(interactionRange);

        #endregion

        #region INTERFACE

        public void AddHealth(int quantity)
        {
            if (godMode) // Para no morirme y poder explorar tranquilo
                return;

            Health = FastMath.Clamp(Health + quantity, 0, maxHealth);
        }

        public void AddOxygen(int quantity)
        {
            Oxygen = FastMath.Clamp(Oxygen + quantity, 0, oxygenCapacity);
        }

        public void CollectItem(Item item)
        {
            Inventory.AddItem(item);
        }

        public void LearnBluePrint(BluePrint bluePrint)
        {
            AvailableBluePrints.Add(bluePrint);
        }

        public bool CanReach(GameObject obj) => IsWithinRange(interactionRange, obj);

        #endregion
    }
}
