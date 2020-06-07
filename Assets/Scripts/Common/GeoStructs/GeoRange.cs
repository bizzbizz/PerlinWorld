using System;
using UnityEngine;

namespace Assets.Scripts.Common.GeoStructs
{
    /// <summary>
    /// Describes corners and size of a spherical rectangle in Spherical Coordinates System
    /// <para/><i>Width and Height are in radians</i>
    /// <para/>The planet as a <see cref="GeoRange"/> has a Width of 2π and Height of π 
    /// </summary>
    [Serializable]
    public struct GeoRange
    {
        [SerializeField]
        public GeoVector Start;
        [SerializeField]
        public GeoVector End;
        [SerializeField]
        public double Width;// = Mathf.PI * 2;
        [SerializeField]
        public double Height;// = Mathf.PI;

        //public GeoRange(GeoVector start, GeoVector end)
        //{
        //    Start = start;
        //    End = end;
        //    Width = end.Tetha - start.Tetha;
        //    Height = end.Phi - start.Phi;
        //}
        /// <summary>
        /// all in radians
        /// </summary>
        /// <param name="start"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public GeoRange(GeoVector start, double width, double height)
        {
            Start = start;
            End = new GeoVector(start.Tetha + width, start.Phi + height);
            Width = width;
            Height = height;
        }
        public GeoRange NextRight()
        {
            return new GeoRange(new GeoVector(End.Tetha, Start.Phi), Width, Height);
        }
        public GeoRange NextBottom()
        {
            return new GeoRange(new GeoVector(Start.Tetha, End.Phi), Width, Height);
        }

        public static GeoRange operator /(GeoRange self, int div)
        {
            return new GeoRange(self.Start, self.Width / div, self.Height / div);
        }

        public override string ToString()
        {
            return string.Format("{0}, [W{1:0.0E0} x H{2:0.0E0}]", Start, Width, Height);
        }
    }

}
