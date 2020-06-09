using System.Collections.Generic;
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
        private readonly QuadTreeBuilder builder;
        private List<TgcBoxDebug> debugQuadTreeBoxes;
        private List<TgcMesh> modelos = new List<TgcMesh>();
        private QuadTreeNode QuadTreeRootNode;
        private TgcBoundingAxisAlignBox sceneBounds;

        public QuadTree()
        {
            builder = new QuadTreeBuilder();
        }

        public void create(List<GameObject> gameObjects, TgcBoundingAxisAlignBox sceneBounds)
        {

            gameObjects.ForEach(el => {
                this.modelos.AddRange(el.Meshes);
            });

            //Deshabilitar todos los mesh inicialmente
            foreach (var mesh in modelos)
            {
                mesh.Enabled = false;
            }
            this.sceneBounds = sceneBounds;

            //Crear QuadTree
            QuadTreeRootNode = builder.crearQuadTree(modelos, sceneBounds);
        }

        /// <summary>
        ///     Crear meshes para debug
        /// </summary>
        public void createDebugQuadTreeMeshes()
        {
            debugQuadTreeBoxes = builder.createDebugQuadTreeMeshes(QuadTreeRootNode, sceneBounds);
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
        }
    }
}