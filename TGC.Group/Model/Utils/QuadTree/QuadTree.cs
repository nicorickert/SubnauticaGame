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
        private List<StaticObject> objetos;
        private QuadTreeNode QuadTreeRootNode;
        private TgcBoundingAxisAlignBox sceneBounds;
        private List<StaticObject> objectsToBeRendered = new List<StaticObject>();

        public QuadTree()
        {
            builder = new QuadTreeBuilder();
        }

        public void create(List<StaticObject> staticObjects, TgcBoundingAxisAlignBox sceneBounds)
        {
            objetos = staticObjects;
            this.sceneBounds = sceneBounds;

            //Crear QuadTree
            QuadTreeRootNode = builder.crearQuadTree(objetos, sceneBounds);
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
        public List<StaticObject> VisibleStaticSceneObjects(TgcFrustum frustum)
        {
            objectsToBeRendered.Clear();

            var pMax = sceneBounds.PMax;
            var pMin = sceneBounds.PMin;
            findVisibleMeshes(frustum, QuadTreeRootNode,
                pMin.X, pMin.Y, pMin.Z,
                pMax.X, pMax.Y, pMax.Z);

            return objectsToBeRendered;
        }

        public void RenderDebugBoxes()
        {
            foreach (var debugBox in debugQuadTreeBoxes)
            {
                debugBox.Render();
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

            // es hoja, cargar todos los meshes
            if (node.esHoja())
            {
                objectsToBeRendered.AddRange(node.objects); //selectLeafMeshes(node);
            }
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
                objectsToBeRendered.AddRange(childNode.objects);//addAllLeafMeshes(childNode);
            }

            //parte adentro: seguir haciendo testeos con hijos
            else if (c == TgcCollisionUtils.FrustumResult.INTERSECT)
            {
                findVisibleMeshes(frustum, childNode, boxLowerX, boxLowerY, boxLowerZ, boxUpperX, boxUpperY, boxUpperZ);
            }
        }

        ///// <summary>
        /////     Hacer visibles todas las meshes de un nodo, buscando recursivamente sus hojas
        ///// </summary>
        //private void addAllLeafMeshes(QuadTreeNode node)
        //{
        //    var children = node.children;

        //    //es hoja, cargar todos los meshes
        //    if (children == null)
        //    {
        //        selectLeafMeshes(node);
        //    }
        //    //pedir hojas a hijos
        //    else
        //    {
        //        for (var i = 0; i < children.Length; i++)
        //        {
        //            addAllLeafMeshes(children[i]);
        //        }
        //    }
        //}

        ///// <summary>
        /////     Hacer visibles todas las meshes de un nodo
        ///// </summary>
        //private void selectLeafMeshes(QuadTreeNode node)
        //{
        //    var objects = node.objects;
        //    foreach (var o in objects)
        //    {
        //        o.Enabled = true;
        //    }
        //}
    }
}