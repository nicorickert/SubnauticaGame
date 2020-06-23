using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Materials
{
    public class Material
    {
        public float Ka { get; private set; } = 0f;
        public float Kd { get; private set; } = 1f;
        public float Ks { get; private set; } = 0f;

        public Material(float ka, float kd, float ks)
        {
            Ka = ka;
            Kd = kd;
            Ks = ks;
        }

        public static Material Metalic => new Material(1f, 1f, 1000000f);

        public static Material Opaque => new Material(0.5f, 1000000f, 0f);
    }
}
