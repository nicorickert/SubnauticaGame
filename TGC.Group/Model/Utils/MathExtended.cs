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
            double sin = v1.X * v2.Y - v2.X * v1.Y;
            double cos = v1.X * v2.X + v1.Y * v2.Y;

            return (float)Math.Atan2(sin, cos);
        }

        public static float AngleBetween(TGCVector3 v1, TGCVector3 v2)
        {
            float cosine = TGCVector3.Dot(v1, v2) / (TGCVector3.Length(v1) * TGCVector3.Length(v2));
            return FastMath.Acos(cosine);
        }

        public static int Sign(float n) => Math.Sign(n);

        public static int GetRandomNumberBetween(int min, int max) => random.Next(min, max);

        public static TGCVector3 Vector3ToTGCVector3(Microsoft.DirectX.Vector3 vector3)
        {
            return new TGCVector3(vector3.X, vector3.Y, vector3.Z);
        }
    }
}
