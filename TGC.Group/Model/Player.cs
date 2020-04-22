using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class Player : GameObject
    {
        /* STATS */
        private int health = 100;
        private int oxygen = 100;
        private float movementSpeed = 130.0f;

        public Player(TGCExample gameInstance, string name) : base(gameInstance, name)
        {
            var loader = new TgcSceneLoader();
            var playerScene = loader.loadSceneFromFile(GameInstance.MediaDir + "Player\\Player-TgcScene.xml");
            Mesh = playerScene.Meshes[0];
            Mesh.Scale = new TGCVector3(0.1f, 0.1f, 0.1f);
            Mesh.Position = new TGCVector3(0, 100, -1000);
            LookDirection = new TGCVector3(0, 0, -1);  // Por como esta orientado el Mesh
        }

        #region GameObject

        public override void Update()
        {
            ManageMovement();
        }

        public override void Render()
        {
            Mesh.Render();
        }

        public override void Dispose()
        {
            Mesh.Dispose();
        }

        #endregion

        #region PRIVATE_METHODS

        private void ChangeHealth(int change)
        {
            health = FastMath.Clamp(health + change, 0, 100);
        }

        private void ChangeOxygen(int change)
        {
            oxygen = FastMath.Clamp(oxygen + change, 0, 100);
        }

        private void ManageMovement()
        {
            TgcD3dInput input = GameInstance.Input;
            TGCVector3 movementDirection = TGCVector3.Empty;

            if (input.keyDown(Key.W))  // Adelante
            {
                movementDirection = LookDirection;
            }
            else if (input.keyDown(Key.S))  // Atras
            {
                movementDirection = -LookDirection;
            }
            else if (input.keyDown(Key.A))  // Izquierda
            {
                TGCMatrix rotationMatrix = TGCMatrix.RotationY(FastMath.PI_HALF);
                movementDirection = ApplyTransformation(rotationMatrix, LookDirection);
            }
            else if (input.keyDown(Key.D))  // Derecha
            {
                TGCMatrix rotationMatrix = TGCMatrix.RotationY(FastMath.PI_HALF * 3);
                movementDirection = ApplyTransformation(rotationMatrix, LookDirection);
            }
            else if (input.keyDown(Key.Space))
            {
                movementDirection = TGCVector3.Up;
            }
            else if (input.keyDown(Key.X))
            {
                movementDirection = -TGCVector3.Up;
            }

            if (!CollisionDetected())
            {
                Mesh.Position += movementDirection * movementSpeed * GameInstance.ElapsedTime;
                Mesh.Transform = TGCMatrix.Translation(Mesh.Position);
            }
        }

        private TGCVector3 ApplyTransformation(TGCMatrix rotationMatrix, TGCVector3 vector)
        {
            TGCVector3 result = TGCVector3.Empty;
            result.X = rotationMatrix.M11 * vector.X + rotationMatrix.M12 * vector.Y + rotationMatrix.M13 * vector.Z;
            result.Y = rotationMatrix.M21 * vector.X + rotationMatrix.M22 * vector.Y + rotationMatrix.M23 * vector.Z;
            result.Z = rotationMatrix.M31 * vector.X + rotationMatrix.M32 * vector.Y + rotationMatrix.M33 * vector.Z;

            return result;
        }

        private bool CollisionDetected() => false;  // TODO

        #endregion

        #region PUBLIC_METHODS

        public void LooseHealth(int loss)
        {
            ChangeHealth(-loss);
        }

        public void GainHealth(int gain)
        {
            ChangeHealth(gain);
        }

        public void LooseOxygen(int loss)
        {
            ChangeOxygen(-loss);
        }

        public void GainOxygen(int gain)
        {
            ChangeOxygen(gain);
        }

        public bool IsDead() => health == 0;

        #endregion
    }
}
