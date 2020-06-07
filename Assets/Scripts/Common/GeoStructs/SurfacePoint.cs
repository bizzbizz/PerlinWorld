using Assets.Scripts.World;
using System;
using UnityEngine;

namespace Assets.Scripts.Common.GeoStructs
{
    [Serializable]
    public struct SurfacePoint
    {
        [SerializeField]
        public bool HasValue;
        public SurfacePoint(VectorXYZ xyz, float value, float noise, PlanetConfig cfg)
        {
            RawNoise = noise;
            Value = value;
            HasValue = true;

            XYZ = xyz;
            Vertice = xyz * (PlanetConfig.PlanetRadius + cfg.PlanetSurfaceMaxHeight * value);
            if (value < cfg.OceanLevel)
                FlatOceanVertice = xyz * (PlanetConfig.PlanetRadius + cfg.PlanetSurfaceMaxHeight * cfg.OceanLevel);
            else FlatOceanVertice = Vertice;
        }

        /// <summary>
        /// Gets the Cartesian location on the surface of a perfect sphere
        /// </summary>
        [SerializeField]
        public VectorXYZ XYZ { get; private set; }

        /// <summary>
        /// gets the Cartesian location on the surface of a planet
        /// </summary>
        [SerializeField]
        public VectorXYZ Vertice { get; private set; }
        [SerializeField]
        public VectorXYZ FlatOceanVertice { get; private set; }

        /// <summary>
        /// gets the perlin noise value
        /// </summary>
        [SerializeField]
        public float Value { get; private set; }

        [SerializeField]
        public float RawNoise { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}={1}", XYZ, Value);
        }
    }
}
