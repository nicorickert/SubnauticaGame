﻿using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Core.Shaders;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Herramienta para crear y utilizar un QuadTree para renderizar por Frustum Culling
    /// </summary>
    public class QuadTree
    {
        private Effect effect;
        private TgcTexture texture;
        private readonly QuadTreeBuilder builder;
        private List<TgcBoxDebug> debugQuadTreeBoxes;
        private List<TgcMesh> modelos = new List<TgcMesh>();
        private List<CustomVertex.PositionNormalTextured> vertices;
        private List<CustomVertex.PositionNormalTextured> verticesToRender = new List<CustomVertex.PositionNormalTextured>();
        private QuadTreeNode QuadTreeRootNode;
        private TgcBoundingAxisAlignBox sceneBounds;

        public QuadTree()
        {
            builder = new QuadTreeBuilder();
        }
        
        public void addGameObjects(List<GameObject> gameObjects)
        {
            gameObjects.ForEach(el => {
                this.modelos.AddRange(el.Meshes);
            });
            //Deshabilitar todos los mesh inicialmente
            foreach (var mesh in modelos)
            {
                mesh.Enabled = false;
            }
        }

        public void create(List<CustomVertex.PositionNormalTextured> vertices, TgcBoundingAxisAlignBox sceneBounds, Effect effect, TgcTexture texture)
        {
            this.vertices = vertices;
            this.sceneBounds = sceneBounds;
            this.effect = effect;
            this.texture = texture;

            //Crear QuadTree
            QuadTreeRootNode = builder.crearQuadTree(modelos, vertices, sceneBounds);
        }

        /// <summary>
        ///     Crear meshes para debug
        /// </summary>
        public void createDebugQuadTreeMeshes()
        {
            debugQuadTreeBoxes = builder.createDebugQuadTreeMeshes(QuadTreeRootNode, sceneBounds);
        }

        public void renderVertexNode(QuadTreeNode node)
        {
            if (node.esHoja())
            {
                if (node.Enabled)
                {
                    node.Render(D3DDevice.Instance.Device, effect, texture);    // Solo le hago render si colisiona con el frustrum
                    node.Enabled = false;
                }
            } else
            {
                foreach(var child in node.children) {
                    renderVertexNode(child);
                }
            }
        }

        /// <summary>
        ///     Renderizar en forma optimizado utilizando el QuadTree para hacer FrustumCulling
        /// </summary>
        public void render(TgcFrustum frustum, bool debugEnabled)
        {
            var pMax = sceneBounds.PMax;
            var pMin = sceneBounds.PMin;
            findVisibleMeshes(frustum, QuadTreeRootNode,
                pMin.X, pMin.Y, pMin.Z,
                pMax.X, pMax.Y, pMax.Z);

            renderVertexNode(QuadTreeRootNode);

            //Renderizar
            foreach (var mesh in modelos)
            {
                if (mesh.Enabled)
                {
                    mesh.Render();
                    mesh.Enabled = false;
                }
            }

            if (debugEnabled)
            {
                foreach (var debugBox in debugQuadTreeBoxes)
                {
                    debugBox.Render();
                }
            }



            /*
            int totalVertices = verticesToRender.Count;
            if (totalVertices == 0) return; // si es 0 explota
            // Render de los triángulos
            var device = D3DDevice.Instance.Device;
            VertexBuffer vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), totalVertices, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            vbTerrain.SetData(verticesToRender.ToArray(), 0, LockFlags.None);

            device.VertexFormat = CustomVertex.PositionNormalTextured.Format; // PositionNormalTextured
            device.SetStreamSource(0, vbTerrain, 0);
            //Render terrain

            if (effect != null && !effect.Disposed)
            {
                // Habilito el canal alpha
                device.RenderState.AlphaTestEnable = true;
                device.RenderState.AlphaBlendEnable = true;

                TGCShaders.Instance.SetShaderMatrixIdentity(effect);
                var numPasses = effect.Begin(0);
                for (var n = 0; n < numPasses; n++)
                {
                    effect.BeginPass(n);
                    D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
                    effect.EndPass();
                }
                effect.End();

                device.RenderState.AlphaTestEnable = false;
                device.RenderState.AlphaBlendEnable = false;

            }
            else
            {
                device.SetTexture(0, texture.D3dTexture);
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
            }

            verticesToRender = new List<CustomVertex.PositionNormalTextured>();
            */
        }

        /// <summary>
        ///     Recorrer recursivamente el QuadTree para encontrar los nodos visibles
        /// </summary>
        private void findVisibleMeshes(TgcFrustum frustum, QuadTreeNode node,
            float boxLowerX, float boxLowerY, float boxLowerZ,
            float boxUpperX, float boxUpperY, float boxUpperZ)
        {
            var children = node.children;

            //es hoja, cargar todos los meshes
            if (children == null)
            {
                selectLeafMeshes(node);
            }

            //recursividad sobre hijos
            else
            {
                var midX = FastMath.Abs((boxUpperX - boxLowerX) / 2);
                var midZ = FastMath.Abs((boxUpperZ - boxLowerZ) / 2);

                //00
                testChildVisibility(frustum, children[0], boxLowerX + midX, boxLowerY, boxLowerZ + midZ, boxUpperX,
                    boxUpperY, boxUpperZ);

                //01
                testChildVisibility(frustum, children[1], boxLowerX + midX, boxLowerY, boxLowerZ, boxUpperX, boxUpperY,
                    boxUpperZ - midZ);

                //10
                testChildVisibility(frustum, children[2], boxLowerX, boxLowerY, boxLowerZ + midZ, boxUpperX - midX,
                    boxUpperY, boxUpperZ);

                //11
                testChildVisibility(frustum, children[3], boxLowerX, boxLowerY, boxLowerZ, boxUpperX - midX, boxUpperY,
                    boxUpperZ - midZ);
            }
        }

        /// <summary>
        ///     Hacer visible las meshes de un nodo si es visible por el Frustum
        /// </summary>
        private void testChildVisibility(TgcFrustum frustum, QuadTreeNode childNode,
            float boxLowerX, float boxLowerY, float boxLowerZ, float boxUpperX, float boxUpperY, float boxUpperZ)
        {
            //test frustum-box intersection
            var caja = new TgcBoundingAxisAlignBox(
                new TGCVector3(boxLowerX, boxLowerY, boxLowerZ),
                new TGCVector3(boxUpperX, boxUpperY, boxUpperZ));
            var c = TgcCollisionUtils.classifyFrustumAABB(frustum, caja);

            //complementamente adentro: cargar todos los hijos directamente, sin testeos
            if (c == TgcCollisionUtils.FrustumResult.INSIDE)
            {
                addAllLeafMeshes(childNode);
            }

            //parte adentro: seguir haciendo testeos con hijos
            else if (c == TgcCollisionUtils.FrustumResult.INTERSECT)
            {
                findVisibleMeshes(frustum, childNode, boxLowerX, boxLowerY, boxLowerZ, boxUpperX, boxUpperY, boxUpperZ);
            }
        }

        /// <summary>
        ///     Hacer visibles todas las meshes de un nodo, buscando recursivamente sus hojas
        /// </summary>
        private void addAllLeafMeshes(QuadTreeNode node)
        {
            var children = node.children;

            //es hoja, cargar todos los meshes
            if (children == null)
            {
                selectLeafMeshes(node);
            }
            //pedir hojas a hijos
            else
            {
                for (var i = 0; i < children.Length; i++)
                {
                    addAllLeafMeshes(children[i]);
                }
            }
        }

        /// <summary>
        ///     Hacer visibles todas las meshes de un nodo
        /// </summary>
        private void selectLeafMeshes(QuadTreeNode node)
        {
            var models = node.models;
            foreach (var m in models)
            {
                m.Enabled = true;
            }
            node.Enabled = true;
        }
    }
}