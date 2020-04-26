using System;
using Microsoft.DirectX.DirectInput;
using System.Diagnostics;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    public class Player : GameObject
    {
        /* STATS */
        private float movementSpeed = 400.0f;
        private float rotationSpeed = 80f;
        private TGCMatrix transformPlayer;

        public Player(Subnautica gameInstance, string name) : base(gameInstance, name)
        {
            var loader = new TgcSceneLoader();
            var playerScene = loader.loadSceneFromFile(GameInstance.MediaDir + "Player\\Player-TgcScene.xml");
            Mesh = playerScene.Meshes[0];
            Mesh.Position = new TGCVector3(0f, 200f, 1200f);
            LookDirection = new TGCVector3(0.5f, 0, 0.5f);  // Por como esta orientado el Mesh originalmente
        }

        #region GameObject

        public override void Update()
        {
            ManageMovement();

            /*
            // Rotacion
            TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0f, 0f), rotation.X);
            TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0f, 1.0f, 0f), rotation.Y);
            TGCQuaternion rotationZ = TGCQuaternion.RotationAxis(new TGCVector3(0f, 0f, 1.0f), rotation.Z);

            TGCQuaternion totalRotation = rotationX * rotationY * rotationZ;
            transformPlayer = baseScale * TGCMatrix.RotationTGCQuaternion(totalRotation) * position; // Roto antes de desplazar para que gire sobre el origen de coords
            */
           

        }

        public override void Render()
        {
            Mesh.Transform = transformPlayer;
            Mesh.Render();
        }

        public override void Dispose()
        {
            Mesh.Dispose();
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
            var moveForward = 0f; // Para detectar si se mueve hacia adelante o hacia atras
            float rotate = 0;

            var lastPos = Mesh.Position;

            if (input.keyDown(Key.W))  // Adelante
            {
                moveForward = -movementSpeed;
            }
            if (input.keyDown(Key.S))  // Atras
            {
                moveForward = movementSpeed;
            }
            if (input.keyDown(Key.A))  // Adelante
            {
                Mesh.Position += ApplyTransformation(TGCMatrix.RotationY(FastMath.PI_HALF), LookDirection) * movementSpeed * GameInstance.ElapsedTime; ;
            }
            if (input.keyDown(Key.D))  // Atras
            {
                Mesh.Position += ApplyTransformation(TGCMatrix.RotationY(FastMath.PI_HALF * 3), LookDirection) * movementSpeed * GameInstance.ElapsedTime; ;
            }
            if (input.keyDown(Key.Q))  // Rotar Izquierda
            {
                rotate -= rotationSpeed;
            }
            if (input.keyDown(Key.E))  // Rotar Derecha
            {
                rotate += rotationSpeed;
            }
            if (input.keyDown(Key.Space))    // De acá para abajo son solo de prueba para poder moverme en la escena libremente
            {
                Mesh.Position += TGCVector3.Up * movementSpeed * GameInstance.ElapsedTime;
            }
            if (input.keyDown(Key.LeftShift))
            {
                Mesh.Position += -TGCVector3.Up * movementSpeed * GameInstance.ElapsedTime;
            }
            

            if (!CollisionDetected())
            {
                //Rotacion
                var rotAngle = Geometry.DegreeToRadian(rotate * GameInstance.ElapsedTime);
                Mesh.Rotation += new TGCVector3(0, rotAngle, 0);

                // Posicion
                var moveF = moveForward * GameInstance.ElapsedTime;
                var z = (float)Math.Cos(Mesh.Rotation.Y) * moveF;
                var x = (float)Math.Sin(Mesh.Rotation.Y) * moveF;
                Mesh.Position += new TGCVector3(x, 0, z); // No pongo Y ya que solo quiero moverme en el plano XZ

                LookDirection = new TGCVector3(-(float)Math.Sin(Mesh.Rotation.Y), 0, -(float)Math.Cos(Mesh.Rotation.Y));

                transformPlayer = TGCMatrix.RotationYawPitchRoll(Mesh.Rotation.Y, Mesh.Rotation.X, Mesh.Rotation.Z) * TGCMatrix.Translation(Mesh.Position);
            }
        }

        // <summary>
        //      Multiplico la matriz por el vector a transformar.
        // <summary>
        private TGCVector3 ApplyTransformation(TGCMatrix rotationMatrix, TGCVector3 vector)
        {
            TGCVector3 result = TGCVector3.Empty;
            result.X = rotationMatrix.M11 * vector.X + rotationMatrix.M12 * vector.Y + rotationMatrix.M13 * vector.Z;
            result.Y = rotationMatrix.M21 * vector.X + rotationMatrix.M22 * vector.Y + rotationMatrix.M23 * vector.Z;
            result.Z = rotationMatrix.M31 * vector.X + rotationMatrix.M32 * vector.Y + rotationMatrix.M33 * vector.Z;

            return result;
        }

        private bool CollisionDetected() => false;  // TODO Tal vez podria ir en la clase GameObject

        #endregion

        #region PUBLIC_METHODS


        #endregion
    }
}
