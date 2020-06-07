using System;
using UnityEngine;

namespace Assets.Scripts.Common.GeoStructs
{
    /// <summary>
    /// Describes a planetary 2D point of a spherical rectangle in Spherical Coordinates System
    /// <para/>Also provides the global 2D index of that point which is less than MaxGlobalSize
    /// </summary>
    [Serializable]
    public struct GeoVector
    {
        // these two must be set before creating any geovector
        internal static long MaxGlobalSizeX;// 32^5 = UnitSize * MaxZoomLevel
        internal static long MaxGlobalSizeY;// 32^5 = UnitSize * MaxZoomLevel

        /// <summary>
        /// Longtitude in radians (W/E)
        /// </summary>
        [SerializeField]
        public double Tetha;
        /// <summary>
        /// Latitude in radians (N/S)
        /// </summary>
        [SerializeField]
        public double Phi;

        /// <summary>
        /// global x index
        /// </summary>
        [SerializeField]
        public long x;
        /// <summary>
        /// global y index
        /// </summary>
        [SerializeField]
        public long y;

        public GeoVector(double tetha, double phi)
        {
            Tetha = tetha;
            Phi = phi;
            x = (long)(Tetha / (2 * Mathf.PI) * MaxGlobalSizeX);
            y = (long)(Phi / Mathf.PI * MaxGlobalSizeY);
        }
        public GeoVector(long x, long y)
        {
            this.x = x;
            this.y = y;
            Tetha = 2 * Mathf.PI / MaxGlobalSizeX * x;
            Phi = Mathf.PI / MaxGlobalSizeY * y;
        }

        public GeoVector NextRight(double dt)
        {
            return new GeoVector(Tetha + dt, Phi);
        }
        public GeoVector NextButtom(double dp)
        {
            return new GeoVector(Tetha, Phi + dp);
        }

        //public static GeoVector operator%(GeoVector self, int v)
        //{
        //    return new WorldGenerator.GeoVector(self.x % v, self.y % v);
        //}
        //public static GeoVector operator /(GeoVector self, int v)
        //{
        //    return new WorldGenerator.GeoVector(self.x / v, self.y / v);
        //}
        /// <summary>
        /// represent in tetha,phi
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("(T{0:0.0E0},P{1:0.0E0})", Tetha, Phi);
        }
    }

}
