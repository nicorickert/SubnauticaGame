using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model
{
    /// <summary>
    ///     Herramienta para construir un QuadTree
    /// </summary>
    internal class QuadTreeBuilder
    {
        //Parametros de corte del QUADTRE
        private readonly int MAX_SECTOR_QuadTree_RECURSION = 2;

        private readonly int MIN_MESH_PER_LEAVE_THRESHOLD = 5; // Sacar para utilizar los triangulos

        public QuadTreeNode crearQuadTree(List<TgcMesh> TgcMeshs, List<CustomVertex.PositionNormalTextured> vertices, TgcBoundingAxisAlignBox sceneBounds)
        {
            var rootNode = new QuadTreeNode();

            //Calcular punto medio y centro
            var midSize = sceneBounds.calculateAxisRadius();
            var center = sceneBounds.calculateBoxCenter();

            //iniciar generacion recursiva de octree
            doSectorQuadTreeX(rootNode, center, midSize, 0, TgcMeshs, vertices);

            return rootNode;
        }

        /// <summary>
        ///     Corte con plano X
        /// </summary>
        private void doSectorQuadTreeX(QuadTreeNode parent, TGCVector3 center, TGCVector3 size,
            int step, List<TgcMesh> meshes, List<CustomVertex.PositionNormalTextured> vertices)
        {
            var x = center.X;

            //Crear listas para realizar corte
            var possitiveList = new List<TgcMesh>();
            var negativeList = new List<TgcMesh>();
            var possitiveVertex = new List<CustomVertex.PositionNormalTextured>();
            var negativeVertex = new List<CustomVertex.PositionNormalTextured>();

            //X-cut
            var xCutPlane = new TGCPlane(1, 0, 0, -x);
            splitByPlane(xCutPlane, meshes, possitiveList, negativeList, vertices, possitiveVertex, negativeVertex);

            //recursividad de positivos con plano Z, usando resultados positivos y childIndex 0
            doSectorQuadTreeZ(parent, new TGCVector3(x + size.X / 2, center.Y, center.Z),
                new TGCVector3(size.X / 2, size.Y, size.Z),
                step, possitiveList, 0, possitiveVertex);

            //recursividad de negativos con plano Z, usando resultados negativos y childIndex 4
            doSectorQuadTreeZ(parent, new TGCVector3(x - size.X / 2, center.Y, center.Z),
                new TGCVector3(size.X / 2, size.Y, size.Z),
                step, negativeList, 2, negativeVertex);
        }

        /// <summary>
        ///     Corte de plano Z
        /// </summary>
        private void doSectorQuadTreeZ(QuadTreeNode parent, TGCVector3 center, TGCVector3 size, int step,
            List<TgcMesh> meshes, int childIndex, List<CustomVertex.PositionNormalTextured> vertices)
        {
            var z = center.Z;

            //Crear listas para realizar corte
            var possitiveList = new List<TgcMesh>();
            var negativeList = new List<TgcMesh>();
            var possitiveVertex = new List<CustomVertex.PositionNormalTextured>();
            var negativeVertex = new List<CustomVertex.PositionNormalTextured>();

            //Z-cut
            var zCutPlane = new TGCPlane(0, 0, 1, -z);
            splitByPlane(zCutPlane, meshes, possitiveList, negativeList, vertices, possitiveVertex, negativeVertex);

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
                posNode.models = possitiveList.ToArray();
                posNode.vertices = possitiveVertex;

                //cargar hijos de nodo negativo
                negNode.models = negativeList.ToArray();
                negNode.vertices = negativeVertex;

                //seguir recursividad
            }
            else
            {
                step++;

                //recursividad de positivos con plano X, usando resultados positivos
                doSectorQuadTreeX(posNode, new TGCVector3(center.X, center.Y, z + size.Z / 2),
                    new TGCVector3(size.X, size.Y, size.Z / 2),
                    step, possitiveList, possitiveVertex);

                //recursividad de negativos con plano Y, usando resultados negativos
                doSectorQuadTreeX(negNode, new TGCVector3(center.X, center.Y, z - size.Z / 2),
                    new TGCVector3(size.X, size.Y, size.Z / 2),
                    step, negativeList, negativeVertex);
            }
        }

        /// <summary>
        ///     Separa los modelos en dos listas, segun el testo contra el plano de corte
        /// </summary>
        private void splitByPlane(TGCPlane cutPlane, List<TgcMesh> modelos,
            List<TgcMesh> possitiveList, List<TgcMesh> negativeList, List<CustomVertex.PositionNormalTextured> vertices,
            List<CustomVertex.PositionNormalTextured> possitiveVertex, List<CustomVertex.PositionNormalTextured> negativeVertex)

        {
            // colision de los modelos 
            TgcCollisionUtils.PlaneBoxResult c;
            foreach (var modelo in modelos)
            {
                c = TgcCollisionUtils.classifyPlaneAABB(cutPlane, modelo.BoundingBox);

                //possitive side
                if (c == TgcCollisionUtils.PlaneBoxResult.IN_FRONT_OF)
                {
                    possitiveList.Add(modelo);
                }

                //negative side
                else if (c == TgcCollisionUtils.PlaneBoxResult.BEHIND)
                {
                    negativeList.Add(modelo);
                }

                //both sides
                else
                {
                    possitiveList.Add(modelo);
                    negativeList.Add(modelo);
                }
            }

            // Colision de los vertices
            TgcCollisionUtils.PointPlaneResult p;
            foreach (var vertice in vertices)
            {
                p = TgcCollisionUtils.classifyPointPlane(MathExtended.Vector3ToTGCVector3(vertice.Position), cutPlane); // Verifico si le vertice está dentro del cuadrante

                //possitive side
                if (p == TgcCollisionUtils.PointPlaneResult.IN_FRONT_OF)
                {
                    possitiveVertex.Add(vertice);
                }

                //negative side
                else if (p == TgcCollisionUtils.PointPlaneResult.BEHIND)
                {
                    negativeVertex.Add(vertice);
                }

                //coincidente
                else
                {
                    possitiveVertex.Add(vertice);
                    negativeVertex.Add(vertice);
                }
            }
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