using TGC.Core.SceneLoader;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;

namespace TGC.Group.Model
{
    class QuadTreeNode
    {
        public QuadTreeNode[] children;
        public TgcMesh[] models;
        public List<CustomVertex.PositionNormalTextured> vertices;

        public bool esHoja()
        {
            return children == null;
        }
    }
}
