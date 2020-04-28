using TGC.Core.Mathematica;
using System;

namespace TGC.Group.Model
{
    public static class MathExtended
    {
        private static Random random = new Random();

        public static TGCVector3 TransformVector3(TGCMatrix transform, TGCVector3 vector)
        {
            TGCVector3 result = TGCVector3.Empty;
            result.X = transform.M11 * vector.X + transform.M12 * vector.Y + transform.M13 * vector.Z;
            result.Y = transform.M21 * vector.X + transform.M22 * vector.Y + transform.M23 * vector.Z;
            result.Z = transform.M31 * vector.X + transform.M32 * vector.Y + transform.M33 * vector.Z;

            return result;
        }

        public static float AngleBetween(TGCVector2 v1, TGCVector2 v2)
        {
            /*
            float dotProduct = TGCVector2.Dot(v1, v2);
            float cosineOfAngle = dotProduct / (TGCVector2.Length(v1) * TGCVector2.Length(v2));
            return FastMath.Acos(cosineOfAngle);
            */
            double sin = v1.X * v2.Y - v2.X * v1.Y;
            double cos = v1.X * v2.X + v1.Y * v2.Y;

            return (float)Math.Atan2(sin, cos);
        }

        public static int GetRandomNumberBetween(int min, int max) => random.Next(min, max);
    }
}
