using Assets.Scripts.Common.Utils;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Assets.Scripts.Common.GeoStructs
{
    [Serializable]
    public class SurfaceArea
    {
        /// <summary>
        /// Corners coordinates and Size in radian
        /// </summary>
        [SerializeField]
        public GeoRange Rect { get; private set; }

        /// <summary>
        /// calculated points
        /// </summary>
        [SerializeField]
        public SurfacePoint[,] Points;
        public SurfaceArea(GeoRange rect)
        {
            Rect = rect;
        }
        public float GetInterpolatedRawNoise(int x, int y, float dx, float dy)
        {
            return MathUtils.Lerp2D(
                dx, dy,
                Points[x, y].RawNoise,
                Points[x + 1, y].RawNoise,
                Points[x, y + 1].RawNoise,
                Points[x + 1, y + 1].RawNoise);

        }
        public SurfacePoint this[int x, int y, int stepSize]
        {
            get
            {
                int X = x * stepSize;
                int Y = y * stepSize;
                return Points[X, Y];
                //if (Points[X, Y].HasValue)
                //    return Points[x, y].Value;
                //else
                //{
                //    int x0 = PlanetConfig.UnitSize * (x / PlanetConfig.UnitSize);
                //    int y0 = PlanetConfig.UnitSize * (y / PlanetConfig.UnitSize);
                //    var v1 = Mathf.Lerp(Points[x0, y0].Value, Points[x0 + 1, y0].Value, (x % PlanetConfig.UnitSize) / (float)PlanetConfig.UnitSize);
                //    var v2 = Mathf.Lerp(Points[x0, y0 + 1].Value, Points[x0 + 1, y0 + 1].Value, (x % PlanetConfig.UnitSize) / (float)PlanetConfig.UnitSize);
                //    return Mathf.Lerp(v1, v2, (y % PlanetConfig.UnitSize) / (float)PlanetConfig.UnitSize);
                //}
            }
        }


        /// <summary>
        /// returns a new angle based on current and given offset
        /// </summary>
        /// <param name="a">between 0 and 1</param>
        /// <returns></returns>
        public float GetTetha(float a)
        {
            return (float)(Rect.Start.Tetha + Rect.Width * a);
        }
        /// <summary>
        /// returns a new angle based on current and given offset
        /// </summary>
        /// <param name="a">between 0 and 1</param>
        /// <returns></returns>
        public float GetPhi(float a)
        {
            return (float)(Rect.Start.Phi + Rect.Height * a);
        }

        public static SurfaceArea Load()
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (!File.Exists(Application.persistentDataPath + "/PlanetC137.sa"))
                return null;
            FileStream file = File.OpenRead(Application.persistentDataPath + "/PlanetC137.sa");
            file.Position = 0;
            var obj = bf.Deserialize(file);
            file.Close();
            return obj as SurfaceArea;
        }
        public void Save()
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/PlanetC137.sa");
            file.Position = 0;
            bf.Serialize(file, this);
            file.Close();
        }
    }
}
