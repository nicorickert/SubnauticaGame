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
        public TgcMesh[] models;
        public List<CustomVertex.PositionNormalTextured> vertices;
        public bool Enabled;

        public bool esHoja()
        {
            return children == null;
        }

        public void Render(Device device, Effect effect, TgcTexture texture)
        {
            int totalVertices = vertices.Count;
            if (totalVertices == 0) return; // si es 0 explota
            // Render de los triángulos
            VertexBuffer vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionNormalTextured), totalVertices, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionNormalTextured.Format, Pool.Default);

            vbTerrain.SetData(vertices.ToArray(), 0, LockFlags.None);

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
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
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
        }
    }
}
