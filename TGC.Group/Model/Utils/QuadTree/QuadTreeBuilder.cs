using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Herramienta para construir un QuadTree
    /// </summary>
    internal class QuadTreeBuilder
    {
        //Parametros de corte del QUADTRE
        private readonly int MAX_SECTOR_QuadTree_RECURSION = 4;

        private readonly int MIN_MESH_PER_LEAVE_THRESHOLD = 5;

        public QuadTreeNode crearQuadTree(List<StaticObject> StaticObjects, TgcBoundingAxisAlignBox sceneBounds)
        {
            var rootNode = new QuadTreeNode();

            //Calcular punto medio y centro
            var midSize = sceneBounds.calculateAxisRadius();
            var center = sceneBounds.calculateBoxCenter();

            //iniciar generacion recursiva de octree
            doSectorQuadTreeX(rootNode, center, midSize, 0, StaticObjects);

            return rootNode;
        }

        /// <summary>
        ///     Corte con plano X
        /// </summary>
        private void doSectorQuadTreeX(QuadTreeNode parent, TGCVector3 center, TGCVector3 size,
            int step, List<StaticObject> meshes)
        {
            var x = center.X;

            //Crear listas para realizar corte
            var possitiveList = new List<StaticObject>();
            var negativeList = new List<StaticObject>();

            //X-cut
            var xCutPlane = new TGCPlane(1, 0, 0, -x);
            splitByPlane(xCutPlane, meshes, possitiveList, negativeList);

            //recursividad de positivos con plano Z, usando resultados positivos y childIndex 0
            doSectorQuadTreeZ(parent, new TGCVector3(x + size.X / 2, center.Y, center.Z),
                new TGCVector3(size.X / 2, size.Y, size.Z),
                step, possitiveList, 0);

            //recursividad de negativos con plano Z, usando resultados negativos y childIndex 4
            doSectorQuadTreeZ(parent, new TGCVector3(x - size.X / 2, center.Y, center.Z),
                new TGCVector3(size.X / 2, size.Y, size.Z),
                step, negativeList, 2);
        }

        /// <summary>
        ///     Corte de plano Z
        /// </summary>
        private void doSectorQuadTreeZ(QuadTreeNode parent, TGCVector3 center, TGCVector3 size, int step,
            List<StaticObject> meshes, int childIndex)
        {
            var z = center.Z;

            //Crear listas para realizar corte
            var possitiveList = new List<StaticObject>();
            var negativeList = new List<StaticObject>();

            //Z-cut
            var zCutPlane = new TGCPlane(0, 0, 1, -z);
            splitByPlane(zCutPlane, meshes, possitiveList, negativeList);

            //obtener lista de children del parent, con iniciacion lazy
            if (parent.children == null)
            {
                parent.children = new QuadTreeNode[4];
            }

            //crear nodo positivo en parent, segun childIndex
            var posNode = new QuadTreeNode();
            parent.children[childIndex] = posNode;

            //cargar nodo negativo en parent, segun childIndex
            var negNode = new QuadTreeNode();
            parent.children[childIndex + 1] = negNode;

            //condicion de corte
            if (step > MAX_SECTOR_QuadTree_RECURSION || meshes.Count < MIN_MESH_PER_LEAVE_THRESHOLD)
            {
                //cargar hijos de nodo positivo
                posNode.objects = possitiveList.ToArray();

                //cargar hijos de nodo negativo
                negNode.objects = negativeList.ToArray();

                //seguir recursividad
            }
            else
            {
                step++;

                //recursividad de positivos con plano X, usando resultados positivos
                doSectorQuadTreeX(posNode, new TGCVector3(center.X, center.Y, z + size.Z / 2),
                    new TGCVector3(size.X, size.Y, size.Z / 2),
                    step, possitiveList);

                //recursividad de negativos con plano Y, usando resultados negativos
                doSectorQuadTreeX(negNode, new TGCVector3(center.X, center.Y, z - size.Z / 2),
                    new TGCVector3(size.X, size.Y, size.Z / 2),
                    step, negativeList);
            }
        }

        /// <summary>
        ///     Separa los objetos en dos listas, segun el testo contra el plano de corte
        /// </summary>
        private void splitByPlane(TGCPlane cutPlane, List<StaticObject> objetos,
            List<StaticObject> possitiveList, List<StaticObject> negativeList)
        {
            TgcCollisionUtils.PlaneBoxResult c;
            foreach (var objeto in objetos)
            {
                c = TgcCollisionUtils.classifyPlaneAABB(cutPlane, objeto.Meshes[0].BoundingBox); // TODO CALCULAR BOUNDING BOX DEL GAMEOBJECT

                //possitive side
                if (c == TgcCollisionUtils.PlaneBoxResult.IN_FRONT_OF)
                {
                    possitiveList.Add(objeto);
                }

                //negative side
                else if (c == TgcCollisionUtils.PlaneBoxResult.BEHIND)
                {
                    negativeList.Add(objeto);
                }

                //both sides
                else
                {
                    possitiveList.Add(objeto);
                    negativeList.Add(objeto);
                }
            }
        }

        /// <summary>
        ///     Se quitan padres cuyos nodos no tengan ningun triangulo
        /// </summary>
        private void optimizeSectorQuadTree(QuadTreeNode[] children)
        {
            if (children == null)
            {
                return;
            }

            for (var i = 0; i < children.Length; i++)
            {
                var childNode = children[i];
                var childNodeChildren = childNode.children;
                if (childNodeChildren != null && hasEmptyChilds(childNode))
                {
                    childNode.children = null;
                    childNode.objects = new StaticObject[0];
                }
                else
                {
                    optimizeSectorQuadTree(childNodeChildren);
                }
            }
        }

        /// <summary>
        ///     Se fija si los hijos de un nodo no tienen mas hijos y no tienen ningun triangulo
        /// </summary>
        private bool hasEmptyChilds(QuadTreeNode node)
        {
            var children = node.children;
            for (var i = 0; i < children.Length; i++)
            {
                var childNode = children[i];
                if (childNode.children != null || childNode.objects.Length > 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Dibujar meshes que representan los sectores del QuadTree
        /// </summary>
        public List<TgcBoxDebug> createDebugQuadTreeMeshes(QuadTreeNode rootNode, TgcBoundingAxisAlignBox sceneBounds)
        {
            var pMax = sceneBounds.PMax;
            var pMin = sceneBounds.PMin;

            var debugBoxes = new List<TgcBoxDebug>();
            doCreateQuadTreeDebugBox(rootNode, debugBoxes,
                pMin.X, pMin.Y, pMin.Z,
                pMax.X, pMax.Y, pMax.Z, 0);

            return debugBoxes;
        }

        private void doCreateQuadTreeDebugBox(QuadTreeNode node, List<TgcBoxDebug> debugBoxes,
            float boxLowerX, float boxLowerY, float boxLowerZ,
            float boxUpperX, float boxUpperY, float boxUpperZ, int step)
        {
            var children = node.children;

            var midX = FastMath.Abs((boxUpperX - boxLowerX) / 2);
            var midZ = FastMath.Abs((boxUpperZ - boxLowerZ) / 2);

            //Crear caja debug
            var box = createDebugBox(boxLowerX, boxLowerY, boxLowerZ, boxUpperX, boxUpperY, boxUpperZ, step);
            debugBoxes.Add(box);

            //es hoja, dibujar caja
            if (children == null)
            {
            }

            //recursividad sobre hijos
            else
            {
                step++;

                //000
                doCreateQuadTreeDebugBox(children[0], debugBoxes, boxLowerX + midX, boxLowerY, boxLowerZ + midZ,
                    boxUpperX, boxUpperY, boxUpperZ, step);
                //001
                doCreateQuadTreeDebugBox(children[1], debugBoxes, boxLowerX + midX, boxLowerY, boxLowerZ, boxUpperX,
                    boxUpperY, boxUpperZ - midZ, step);

                //100
                doCreateQuadTreeDebugBox(children[2], debugBoxes, boxLowerX, boxLowerY, boxLowerZ + midZ,
                    boxUpperX - midX, boxUpperY, boxUpperZ, step);
                //101
                doCreateQuadTreeDebugBox(children[3], debugBoxes, boxLowerX, boxLowerY, boxLowerZ, boxUpperX - midX,
                    boxUpperY, boxUpperZ - midZ, step);
            }
        }

        /// <summary>
        ///     Construir caja debug
        /// </summary>
        private TgcBoxDebug createDebugBox(float boxLowerX, float boxLowerY, float boxLowerZ,
            float boxUpperX, float boxUpperY, float boxUpperZ, int step)
        {
            //Determinar color y grosor según profundidad
            Color c;
            float thickness;
            switch (step)
            {
                case 0:
                    c = Color.Red;
                    thickness = 4f;
                    break;

                case 1:
                    c = Color.Violet;
                    thickness = 3f;
                    break;

                case 2:
                    c = Color.Brown;
                    thickness = 2f;
                    break;

                case 3:
                    c = Color.Gold;
                    thickness = 1f;
                    break;

                default:
                    c = Color.Orange;
                    thickness = 0.5f;
                    break;
            }

            //Crear caja Debug
            var box = TgcBoxDebug.fromExtremes(
                new TGCVector3(boxLowerX, boxLowerY, boxLowerZ),
                new TGCVector3(boxUpperX, boxUpperY, boxUpperZ),
                c, thickness);

            return box;
        }
    }
}