using TGC.Core.SceneLoader;
using Microsoft.DirectX.Direct3D;
using System.Collections.Generic;
using TGC.Core.Shaders;
using TGC.Core.Textures;

namespace TGC.Group.Model
{
    public class QuadTreeNode
    {
        public QuadTreeNode[] children;
        public Collectable[] collectables;
        public List<CustomVertex.PositionNormalTextured> vertices;
        public bool Enabled;

        public bool esHoja()
        {
            return children == null;
        }
    }
}
