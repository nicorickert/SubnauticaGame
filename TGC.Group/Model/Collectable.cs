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
    class Collectable : StaticObject
    {
        private EItemID resourceID;
        private readonly float regenerationCooldown = 20f;
        private float timeSinceInactive = 0f;
        private bool isActive = true;

        #region CONSTRUCTORES

        public Collectable(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation, EItemID resourceID) 
            : base(gameInstance, name, meshes, position, scale, rotation)
        {
            this.resourceID = resourceID;
        }

        public Collectable(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, float scale, TGCVector3 rotation, EItemID resourceID)
            : this(gameInstance, name, meshes, position, TGCVector3.One * scale, rotation, resourceID) { }

        public Collectable(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, float scale, EItemID resourceID)
            : this(gameInstance, name, meshes, position, TGCVector3.One * scale, TGCVector3.Empty, resourceID) { }

        public Collectable(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, TGCVector3 rotation, EItemID resourceID)
            : this(gameInstance, name, meshes, position, TGCVector3.One, rotation, resourceID) { }

        public Collectable(Subnautica gameInstance, string name, List<TgcMesh> meshes, TGCVector3 position, EItemID resourceID)
            : this(gameInstance, name, meshes, position, TGCVector3.One, TGCVector3.Empty, resourceID) { }

        #endregion

        public override void Update()
        {
            base.Update();

            if (!isActive)
            {
                timeSinceInactive += GameInstance.ElapsedTime;

                if (regenerationCooldown <= timeSinceInactive)
                    Activate();
            }
        }

        public override void Render()
        {
            if(isActive)
                base.Render();
        }

        public override void Interact(Player interactor)
        {
            if (isActive)
            {
                interactor.CollectItem(ItemDatabase.Instance.Generate(resourceID));
                GameInstance.CoralpickupSound.play();
                Deactivate();
            }
        }

        private void Activate()
        {
            isActive = true;
        }

        private void Deactivate()
        {
            isActive = false;
            timeSinceInactive = 0f;
        }
    }
}
