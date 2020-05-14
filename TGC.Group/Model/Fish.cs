using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;
using System.Collections.Generic;
using TGC.Group.Model.Items;

namespace TGC.Group.Model
{
    class Fish : Roamer
    {
        #region STATS
        private float size;
        #endregion

        public Fish(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 spawnLocation) : base(gameInstance, name, meshes, 50f)
        {
            CollisionStatus = Utils.ECollisionStatus.NOT_COLLISIONABLE;
            Position = spawnLocation;
            size = MathExtended.GetRandomNumberBetween(3, 20);
            scale = TGCVector3.One * size;
        }

        #region TGC
        public override void Update()
        {
            Roam();
        }
        #endregion

        #region INTERFACE
        public override void Interact(Player interactor)
        {
            interactor.CollectItem(new Food("raw_fish", "un spritepath", 20)); // ojo, arreglar lo del sprite path
            Destroy();
        }
        #endregion
    }
}
