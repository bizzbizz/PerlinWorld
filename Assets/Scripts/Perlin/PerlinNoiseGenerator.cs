using Assets.Scripts.Common.GeoStructs;
using Assets.Scripts.World;
using UnityEngine;

namespace Assets.Scripts.Perlin
{
    public static class PerlinNoiseGenerator
    {

        #region Perlin 3D, Perlin 4D
        public static float PerlinNoise(float nx, float ny, float nz, float nw)
        {
            return
                (Mathf.PerlinNoise(nx, ny)
                + Mathf.PerlinNoise(nx, nz)
                + Mathf.PerlinNoise(nx, nw)

                + Mathf.PerlinNoise(ny, nx)
                + Mathf.PerlinNoise(ny, nz)
                + Mathf.PerlinNoise(ny, nw)

                + Mathf.PerlinNoise(nz, nx)
                + Mathf.PerlinNoise(nz, ny)
                + Mathf.PerlinNoise(nz, nw)

                + Mathf.PerlinNoise(nw, nx)
                + Mathf.PerlinNoise(nw, ny)
                + Mathf.PerlinNoise(nw, nz)) / 12;
        }
        public static float PerlinNoise(VectorXYZ vec)
        {
            return
                (Mathf.PerlinNoise(vec.x, vec.y)
                + Mathf.PerlinNoise(vec.x, vec.z)
                + Mathf.PerlinNoise(vec.y, vec.z)
                + Mathf.PerlinNoise(vec.y, vec.x)
                + Mathf.PerlinNoise(vec.z, vec.y)
                + Mathf.PerlinNoise(vec.z, vec.x)) / 6;
        }

        internal static SurfacePoint SurfacePerlinNoise(VectorXYZ xyz, PlanetConfig cfg, int extraLod = 0)
        {
            float noise = 0;
            for (int i = 0; i < cfg.LOD + extraLod; i++)
            {
                noise += (PerlinNoise(xyz * cfg.LodConfigs[i].frequency + cfg.NoiseOffset) * cfg.LodConfigs[i].magnitude);
            }
            float ppnoise = cfg.PostProcess(noise);
            return new SurfacePoint(xyz, ppnoise, noise, cfg);
        }
        #endregion

        #region Noise Calculation

        /// <summary>
        /// calculate noise on the surface of the planet
        /// </summary>
        public static SurfacePoint[,] CreatePlanetSurfacePoints(PlanetConfig cfg)
        {
            int X = 1 + PlanetConfig.PointSize;
            int Y = 1 + PlanetConfig.PointSize / 2;
            var surfacePoints = new SurfacePoint[X, Y];

            var sp = new float[Y];
            var cp = new float[Y];
            for (int p = 0; p < Y; p++)
            {
                var phi = Mathf.PI * 2 * p / PlanetConfig.PointSize;
                sp[p] = Mathf.Sin(phi);
                cp[p] = Mathf.Cos(phi);
            }

            for (int t = 0; t < X; t++)
            {
                //for faster calc
                var tetha = Mathf.PI * 2 * t / PlanetConfig.PointSize;
                float st = Mathf.Sin(tetha);
                float ct = Mathf.Cos(tetha);

                for (int p = 0; p < Y; p++)
                {
                    //var phi = (Mathf.PI * 2) * (float)p / PlanetConfig.PointSize;
                    //float sp = Mathf.Sin(phi);
                    //float cp = Mathf.Cos(phi);

                    //calc spherical positions
                    var xyz = new VectorXYZ(sp[p] * ct, cp[p], sp[p] * st);
                    surfacePoints[t, p] = SurfacePerlinNoise(xyz, cfg);
                }
            }

            return surfacePoints;
        }

        /// <summary>
        /// calculate noise on the surface of the planet
        /// </summary>
        public static void CreateTerrainSurfacePoints(this SurfaceArea surfaceArea, PlanetConfig cfg, int extraLevelOfDetail)
        {
            surfaceArea.Points = new SurfacePoint[PlanetConfig.PointSize, PlanetConfig.PointSize];
            int size = PlanetConfig.PointSize;// (mode == UnitRenderer.RenderMode.Good ? PlanetConfig.PointSize : PlanetConfig.LowPolyTexSize);

            var sp = new float[size];
            var cp = new float[size];
            for (int p = 0; p < size; p++)
            {
                var phi = surfaceArea.GetPhi((float)p / size);
                sp[p] = Mathf.Sin(phi);
                cp[p] = Mathf.Cos(phi);
            }

            for (int t = 0; t < size; t++)
            {
                //for faster calc
                float tetha = surfaceArea.GetTetha((float)t / size);
                float st = Mathf.Sin(tetha);
                float ct = Mathf.Cos(tetha);

                for (int p = 0; p < size; p++)
                {
                    //float phi = Surface.GetPhi((float)p / size);
                    //float sp = Mathf.Sin(phi);
                    //float cp = Mathf.Cos(phi);

                    //calc spherical positions
                    var xyz = new VectorXYZ(sp[p] * ct, cp[p], sp[p] * st);
                    surfaceArea.Points[t, p] = PerlinNoiseGenerator.SurfacePerlinNoise(xyz, cfg, extraLevelOfDetail);
                }
            }
        }

        #endregion

    }
}
