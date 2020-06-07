using Assets.Scripts.Common.GeoStructs;
using Assets.Scripts.Perlin;
using UnityEngine;
namespace Assets.Scripts.World
{
    [System.Serializable]
    public class PlanetConfig
    {
        [Header("Noise Data")]
        public NoiseConfig PlanetNoiseConfig;
        internal NoiseConfig[] LodConfigs;
        public Vector3Int NoiseOffset;

        [Header("Zoom and Details")]
        public int LOD = 4;

        /// <summary>
        /// the number of divisions in each dimension of each unit
        /// <para>only the planet splits into [unitSize, unitSize/2]</para>
        /// <para>also = xSizes[last]</para>
        /// </summary>
        public const int UnitSize = 2;

        public int MaxZoomLevel = 4;
        public int MaxDetailLevel = 7;


        public const int PointLog = 7;
        public const int PointSize = 128;

        public const int zoomGenerationRadius = 1;//all lowpoly. except for center which is rendered in best mode

        internal long[] xSizes;//UnitSize^N, ..., UnitSize^0
        internal long[] ySizes;//32^5, 32^4, ..., 32

        public const float PlanetRadius = 1000f;
        [Range(1f, 10000f)]
        public float PlanetSurfaceMaxHeight = 1200f;

        [Range(0, .25f), Header("%")]
        public float DeepestLevel;
        [Range(0.25f, .75f)]
        public float OceanLevel;
        public const float HillLevel = 0.75f;
        [Range(.75f, 1)]
        public float SummitLevel;

        //[Range(0, 50)]
        //public int polarHeightRatioH = 10;//unwanted height in percent

        //internal int polarHeight;//polar height pixels (mostly out of sphere range)


        public void InitPlanetConfig()
        {

            //setup config

            LodConfigs = new NoiseConfig[LOD + MaxZoomLevel];
            LodConfigs[0] = PlanetNoiseConfig;
            for (int i = 1; i < LodConfigs.Length; i++)
            {
                LodConfigs[i] = LodConfigs[i - 1];
                LodConfigs[i].frequency *= 2;
                LodConfigs[i].magnitude /= 2;
            }

            xSizes = new long[MaxDetailLevel];
            ySizes = new long[MaxDetailLevel];
            var max = 1L;
            for (int i = MaxDetailLevel - 1; i >= 0; i--)
            {
                max *= (long)UnitSize;
                xSizes[i] = max;
                ySizes[i] = max;
            }
            ySizes[0] /= 2L;

            //setup GeoVector
            GeoVector.MaxGlobalSizeX = max;
            GeoVector.MaxGlobalSizeY = max / 2;


            //            //polarHeight = PlanetPointSize / polarHeightRatioH;
            UnitRenderer.deepestLevel = DeepestLevel;
            UnitRenderer.oceanLevel = OceanLevel;
            UnitRenderer.hillLevel = HillLevel + (HillLevel - OceanLevel);
            UnitRenderer.summitLevel = SummitLevel + (SummitLevel - HillLevel) * 4 + (HillLevel - OceanLevel);

        }

        #region Noise Gen Filters
        public float PostProcess(float n)
        {
            int level = (n >= SummitLevel) ? 3 :
                    ((n >= HillLevel) ? 2 :
                    ((n > OceanLevel) ? 1 :
                    0));

            return GetRaiseGroundValue(n, level);
        }
        float GetRaiseGroundValue(float n, int level)
        {
            switch (level)
            {
                default:
                case 0: return n;
                case 1: return OceanLevel + (n - OceanLevel) * 2;
                case 2: return UnitRenderer.hillLevel + (n - HillLevel) * 5;
                case 3: return UnitRenderer.summitLevel + (n - SummitLevel);
            }
        }
        //void GetWrapPolarCorners()
        //{
        //    //poles
        //    for (int y = 0; y < polarHeight; y++)
        //    {
        //        float strength = y / polarHeight;
        //        //wrap around
        //        for (int x = 0; x < PlanetSize; x++)
        //        {
        //            //north pole
        //            Points[x, y].Value *= strength;
        //            //south pole
        //            Points[x, PlanetSize - y - 1].Value *= strength;
        //        }
        //    }
        //} 
        #endregion
    }
}
