using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils
{
    public static class MathExtended
    {
        public static float NormOfVector3(TGCVector3 v) => FastMath.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);

        public static TGCVector3 TransformVector3(TGCMatrix transform, TGCVector3 vector)
        {
            TGCVector3 result = TGCVector3.Empty;
            result.X = transform.M11 * vector.X + transform.M12 * vector.Y + transform.M13 * vector.Z;
            result.Y = transform.M21 * vector.X + transform.M22 * vector.Y + transform.M23 * vector.Z;
            result.Z = transform.M31 * vector.X + transform.M32 * vector.Y + transform.M33 * vector.Z;

            return result;
        }
    }
}
