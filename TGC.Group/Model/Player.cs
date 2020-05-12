using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Utils;

namespace TGC.Group.Model
{
    public class Player : GameObject
    {
        private readonly float timePerHitTick = 0.5f;
        private float timeSinceLastTick = 0f;

        /* STATS */
        private readonly float movementSpeed = 1000.0f;
        private int maxHealth = 100;
        private int oxygenCapacity = 100;
        private int health = 100;
        private int oxygen = 100;
        private bool IsAlive { get { return health > 0; } }
        private bool IsOutOfOxygen { get { return oxygen == 0; } }

        public Player(Subnautica gameInstance, string name, List<TgcMesh> meshes) : base(gameInstance, name, meshes)
        {
            Position = new TGCVector3(0, 100, 2000);
        }

        #region GameObject

        public override void Update()
        {
            Transform = TGCMatrix.Identity;

            FixRotation();

            if (IsAlive)
            {
                ManageMovement();
                UpdateVitals();
            }
        }

        public override void Render()
        {
            foreach (TgcMesh mesh in Meshes)
            {
                mesh.Transform = Transform;
                mesh.Render();
            }
        }

        public override void Dispose()
        {
            foreach (TgcMesh mesh in Meshes)
                mesh.Dispose();
        }

        #endregion

        #region PRIVATE_METHODS

        // <summary>
        //      Dado un input WASD el Mesh se mueve a la izquierda, derecha, adelante y atras respecto del vector LookDirection.
        // <summary>
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

            if (!CollisionDetected())
            {
                TGCVector3 totalTranslation = TGCVector3.Normalize(movementDirection) * movementSpeed * GameInstance.ElapsedTime;
                Position += totalTranslation;

                TGCMatrix translationMatrix = TGCMatrix.Translation(Position);
                Transform *= translationMatrix;
            }
        }

        private bool CollisionDetected() => false;  // TODO Tal vez podria ir en la clase GameObject

        private void FixRotation()
        {
            TGCVector3 rotationAxis = TGCVector3.Cross(InitialLookDirection, LookDirection);  // Ojo el orden - no es conmutativo
            TGCQuaternion rotation = TGCQuaternion.RotationAxis(rotationAxis, MathExtended.AngleBetween(InitialLookDirection, LookDirection));
            TGCMatrix rotationMatrix = TGCMatrix.RotationTGCQuaternion(rotation);
            Transform *= rotationMatrix;  // TODO Ver cuando estén las colisiones si hay que hacer la rotacion respecto de la cabeza o desde los pies (actualmente desde los pies)
        }

        private bool IsSubmerged() => Position.Y < GameInstance.WaterY;

        private void AddHealth(int quantity)
        {
            health = FastMath.Clamp(health + quantity, 0, maxHealth);
        }

        private void AddOxygen(int quantity)
        {
            oxygen = FastMath.Clamp(oxygen + quantity, 0, oxygenCapacity);
        }

        private void UpdateVitals()
        {
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
                    oxygen = oxygenCapacity;
                }

                System.Console.WriteLine("Player health: " + health + " oxygen: " + oxygen);
                timeSinceLastTick = 0;
            }

        }

        #endregion
    }
}
