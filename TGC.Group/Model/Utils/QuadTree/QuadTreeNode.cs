using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class QuadTreeNode
    {
        public QuadTreeNode[] children;
        public TgcMesh[] models;

        public bool esHoja()
        {
            return children == null;
        }
    }
}
