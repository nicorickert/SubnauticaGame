using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using System.Collections.Generic;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    class Fish : Roamer
    {
        private readonly float size;

        public Fish(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 spawnLocation) : base(gameInstance, name, meshes, 10, 50f)
        {
            CollisionStatus = Utils.ECollisionStatus.NOT_COLLISIONABLE;
            Position = spawnLocation;
            size = MathExtended.GetRandomNumberBetween(3, 20);
            scale = TGCVector3.One * size;

            // LIVING_NPC

            dropsProbability[ItemDatabase.Instance.Generate(EItemID.RAW_FISH)] = 100;
            dropsProbability[ItemDatabase.Instance.Generate(EItemID.FISH_SCALE)] = 50;
        }

        #region TGC
        public override void Update() { }

        public override void Render()
        {
            Roam();
            base.Render();
        }
        #endregion
    }
}
