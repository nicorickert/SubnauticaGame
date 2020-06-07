﻿using System;
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

        public int XZRadius { get => (int)currentScaleXZ * verticesWidth / 2; }
        public int YMax { get => (int)GameInstance.FloorLevelToWorldHeight(0) + 1500; } // Mas o menos para no calcularlo


        public SueloDelMar(Subnautica gameInstance, string name, TGCVector3 centreP, string heightMap, string texture, string effect) : base(gameInstance, name, centreP, heightMap, texture, effect)
        {
            
        }

        // agarrar los vertices y utilizarlos para agregar los elementos del terreno
        public override void Init()
        {
            base.Init();
            InitMainMeshes();
            CrearObjetosEnElEscenario(vertices.ToArray());

            quadtree = new QuadTree();
            quadtree.create(instancesPlantas, new Core.BoundingVolumes.TgcBoundingAxisAlignBox(centre - new TGCVector3(XZRadius, -4000, XZRadius), centre + new TGCVector3(XZRadius, YMax, XZRadius)));
            quadtree.createDebugQuadTreeMeshes();
        }

        public override void Render()
        {
            base.Render();
            quadtree.render(GameInstance.Frustum, true);
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
            int posicionesTotales = (verticesWidth - 2) * (verticesHeight - 2); // Le resto 2 para que no tener en cuenta los bordes del mapa
            int posicionesASaltear = 1; // Este valor se cambia adentro del for con un random
            int minSalto = 5; // Valores para usar en el next del random para saltear
            int maxSalto = 10;
            
            for (int i = verticesWidth; i < posicionesTotales; i += posicionesASaltear)
            {
                CustomVertex.PositionNormalTextured verticeActual = vertices[i * 6 + 1]; // Cada 6 es un vértice de otro triángulo
                TGCVector3 rotation = TGCVector3.Up * random.Next(10);
                int scale = random.Next(10, 30);
                TGCVector3 position = MathExtended.Vector3ToTGCVector3(verticeActual.Position);
                TgcMesh mesh = meshesPlantas[random.Next(meshesPlantas.Count)];


               instancesPlantas.Add(new Collectable(GameInstance, "coral", new List<TgcMesh>(new TgcMesh[] { mesh.createMeshInstance("coral") }), position, scale, rotation, Items.EItemID.CORAL_PIECE));
                posicionesASaltear = random.Next(minSalto, maxSalto);
            }
        }
        #endregion
    }
}
