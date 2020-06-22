using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Utils.Materiales
{
    class Material
    {
        public float Ka { get; }
        public float Kd { get; }
        public float Ks { get; }

        public Material Default { get => new Material(0, 1, 0); }


        public Material(float ka, float kd, float ks)
        {
            Ka = ka;
            Kd = kd;
            Ks = ks;
        }
    }
}
