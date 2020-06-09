using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class SueloDelMar : HeightMapTextured 
    {

        private List<TgcMesh> meshesPlantas = new List<TgcMesh>();
        private List<GameObject> instancesPlantas = new List<GameObject>();
        private QuadTree quadtree;

        public SueloDelMar(Subnautica gameInstance, string name, TGCVector3 centreP, string heightMap, string texture, string effect, float scaleXZ, float scaleY) : base(gameInstance, name, centreP, heightMap, texture, effect, scaleXZ, scaleY)
        {
            
        }

        // agarrar los vertices y utilizarlos para agregar los elementos del terreno
        public override void Init()
        {
            base.Init();

            InitMainMeshes();
            CrearObjetosEnElEscenario(vertices.ToArray());
            
        }

        #region PRIVATE METHODS
        private void InitMainMeshes()
        {
            var loader = new TgcSceneLoader();

            // Agrego los meshes a generar
            //meshesPlantas.Add(loader.loadSceneFromFile(GameInstance.MediaDir + "Aquatic\\Meshes\\coral-TgcScene.xml").Meshes[0]);
            meshesPlantas.Add(loader.loadSceneFromFile(GameInstance.MediaDir + "Aquatic\\Meshes\\brain_coral-TgcScene.xml").Meshes[0]);
            meshesPlantas.Add(loader.loadSceneFromFile(GameInstance.MediaDir + "Aquatic\\Meshes\\pillar_coral-TgcScene.xml").Meshes[0]);
            meshesPlantas.Add(loader.loadSceneFromFile(GameInstance.MediaDir + "Aquatic\\Meshes\\spiral_wire_coral-TgcScene.xml").Meshes[0]);
            meshesPlantas.Add(loader.loadSceneFromFile(GameInstance.MediaDir + "Aquatic\\Meshes\\tree_coral-TgcScene.xml").Meshes[0]);
        }

        private void CrearObjetosEnElEscenario(CustomVertex.PositionNormalTextured[] vertices)
        {
            Random random = new Random();
            int posicionesTotales = vertices.Length; // Le resto 2 para que no tener en cuenta los bordes del mapa
            int posicionesASaltear = 1; // Este valor se cambia adentro del for con un random
            int minSalto = 25; // Valores para usar en el next del random para saltear
            int maxSalto = 50;
            
            for (int i = verticesWidth; i < posicionesTotales; i += posicionesASaltear)
            {
                CustomVertex.PositionNormalTextured verticeActual = vertices[i];
                TGCVector3 rotation = TGCVector3.Up * random.Next(10);
                int scale = random.Next(10, 30);
                TGCVector3 position = MathExtended.Vector3ToTGCVector3(verticeActual.Position);
                TgcMesh mesh = meshesPlantas[random.Next(meshesPlantas.Count)];

                //GameInstance.InstanceObject(new Collectable(GameInstance, "coral", new List<TgcMesh>(new TgcMesh[] { mesh.createMeshInstance("coral") }), position, scale, rotation, Items.EItemID.CORAL_PIECE));
                GameInstance.InstanceStaticSceneObject(new Collectable(GameInstance, "coral", new List<TgcMesh>(new TgcMesh[] { mesh.createMeshInstance("coral") }), position, scale, rotation, Items.EItemID.CORAL_PIECE));
                posicionesASaltear = random.Next(minSalto, maxSalto);
            }
        }
        #endregion
    }
}
