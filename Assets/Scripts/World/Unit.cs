#define DEBPRINT
using Assets.Scripts.Common.GeoStructs;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Perlin;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Unit
    {
        internal PlanetConfig cfg;
        internal UnitRenderer unitRenderer;

        internal Vector2Int dataStart;

        /// <summary>
        /// 0:Planet, 1:MegaTerrain, ...
        /// </summary>
        public int ZoomLevel;
        /// <summary>
        /// -1 is not calculated, level 0 is fully calculated. more than 0 is the distance form nearest level0 upward
        /// </summary>
        public int CalcLevel
        {
            get
            {
                if (Surface.Points != null)
                    return 0;
                else if (ZoomLevel == 0)
                {
                    return -1;
                }
                else
                    return Parent.CalcLevel + 1;
            }
        }


        public Unit Parent;
        Unit[,] _units;
        public Vector2Int LocalIndex { get; private set; }

        /// <summary>
        /// Data Access
        /// </summary>
        public SurfaceArea Surface { get; private set; }

        /// <summary>
        /// returns a parent(|self) unit with surface points
        /// </summary>
        /// <returns></returns>
        public Unit GetDataUnit()
        {
            switch (CalcLevel)
            {
                case 0:
                    return this;
                case -1:
                    return null;
                default:
                    return Parent.GetDataUnit();
            }
        }

        #region Unit Access
        public Unit GetUnit(int x, int y)
        {
            if (_units == null)
            {
                Debug.Log("creating units @ZL" + ZoomLevel + " @" + x + " " + y);
                CreateUnits();
            }

            if (ZoomLevel == 0)
            {
                //wrap planet units
                //the planet divides into UnitSize/2 units on its height
                return _units[x.MOD(PlanetConfig.UnitSize), y.MOD(PlanetConfig.UnitSize / 2)];
            }

            //find other units
            if (x < 0)
                return Parent.GetUnit(LocalIndex.x - 1, LocalIndex.y).GetUnit(x + PlanetConfig.UnitSize, y);
            if (x >= PlanetConfig.UnitSize)
                return Parent.GetUnit(LocalIndex.x + 1, LocalIndex.y).GetUnit(x - PlanetConfig.UnitSize, y);
            if (y < 0)
                return Parent.GetUnit(LocalIndex.x, LocalIndex.y - 1).GetUnit(x, y + PlanetConfig.UnitSize);
            if (y >= PlanetConfig.UnitSize)
                return Parent.GetUnit(LocalIndex.x, LocalIndex.y + 1).GetUnit(x, y - PlanetConfig.UnitSize);

            return _units[x, y];
        }
        public Vector2Int GetLocalUnitIndex(GeoVector position)
        {
            int currentIndex = ZoomLevel;
            int x = (int)(position.x % cfg.xSizes[currentIndex] / cfg.xSizes[currentIndex + 1]);
            int y = (int)(position.y % cfg.ySizes[currentIndex] / cfg.ySizes[currentIndex + 1]);
            return new Vector2Int(x, y);
        }
        //public float GetGeneratedValue(GeoVector position)
        //{
        //    if (!IsCalculated)
        //        return 0;
        //    int currentIndex = (int)ZoomLevel;
        //    int x = (int)(PlanetConfig.PointSize * (position.x % cfg.globalSizesX[currentIndex]) / cfg.globalSizesX[currentIndex]);
        //    int y = (int)(PlanetConfig.PointSize * (position.y % cfg.globalSizesY[currentIndex]) / cfg.globalSizesY[currentIndex]);
        //    if(Surface.Points[x,y].HasValue)
        //    {
        //        if(Surface.Points[x, y].is)
        //    }
        //    return Surface[x, y];
        //}
        #endregion



        //---------------------------------------------------------------factory methods--------------------------------------------------------------------------------------
        public static Unit CreatePlanetUnit(PlanetConfig cfg)
        {
            var unit = CreateUnit(cfg,
                Vector2Int.zero,
                new GeoRange(new GeoVector(0, 0), Mathf.PI * 2, Mathf.PI),
                null,
                0);
            unit.CreatePlanetUnits();
            return unit;
        }
        static Unit CreateUnit(
            PlanetConfig cfg,
            Vector2Int localIdx,
            GeoRange rect,
            Unit parentUnit,
            int zoomLevel)
        {
            //var unit = new GameObject().AddComponent<Unit>();
            //unit.name = localIdx.ToString();
            //unit.transform.parent = parentGo;
            var unit = new Unit
            {
                Parent = parentUnit,
                cfg = cfg,
                ZoomLevel = zoomLevel
            };
            if (parentUnit == null)
                unit.dataStart = localIdx;
            else
                unit.dataStart = parentUnit.dataStart * PlanetConfig.UnitSize + localIdx;
            unit.LocalIndex = localIdx;
            if (zoomLevel == 0)
            {
                //unit.Surface = SurfaceArea.Load();
                if (unit.Surface == null)
                {
                    unit.Surface = new SurfaceArea(rect);
                    unit.Surface.Points = PerlinNoiseGenerator.CreatePlanetSurfacePoints(cfg);
                    //unit.Surface.Save();
                }
            }
            else
                unit.Surface = new SurfaceArea(rect);

            return unit;
        }

        void CreatePlanetUnits()
        {
            _units = new Unit[PlanetConfig.UnitSize, PlanetConfig.UnitSize / 2];
            GeoRange rect = new GeoRange(Surface.Rect.Start, Surface.Rect.Width / PlanetConfig.UnitSize, 2 * Surface.Rect.Height / PlanetConfig.UnitSize);
            for (int x = 0; x < PlanetConfig.UnitSize; x++)
            {
                GeoRange tmpRect = rect;
                for (int y = 0; y < PlanetConfig.UnitSize / 2; y++)
                {
                    _units[x, y] = CreateUnit(cfg, new Vector2Int(x, y), rect, this, ZoomLevel + 1);

                    rect = rect.NextBottom();
                }
                rect = tmpRect.NextRight();
            }
        }
        void CreateUnits()
        {
            _units = new Unit[PlanetConfig.UnitSize, PlanetConfig.UnitSize];
            GeoRange rect = Surface.Rect / PlanetConfig.UnitSize;
            for (int x = 0; x < PlanetConfig.UnitSize; x++)
            {
                GeoRange tmpRect = rect;
                for (int y = 0; y < PlanetConfig.UnitSize; y++)
                {
                    _units[x, y] = CreateUnit(cfg, new Vector2Int(x, y), rect, this, ZoomLevel + 1);

                    rect = rect.NextBottom();
                }
                rect = tmpRect.NextRight();
            }
        }


        internal void Render(int renderLevel)
        {
            //create renderer
            if (unitRenderer == null)
            {
                unitRenderer = PlanetGenerator.CreateRenderer();
                string name = LocalIndex.ToString();
                Unit tmp = Parent;
                while (tmp != null && tmp.Parent != null)
                {
                    name = tmp.LocalIndex + " - " + name;
                    tmp = tmp.Parent;
                }
                unitRenderer.name = name;
            }

            //generate noise now
            if (CalcLevel == -1)
            {
                Surface.CreateTerrainSurfacePoints(cfg, ZoomLevel);
            }
            else if (PlanetConfig.PointLog - CalcLevel < renderLevel)
            {
                Surface.CreateTerrainSurfacePoints(cfg, ZoomLevel);
            }

            //render
            if (unitRenderer.RenderLevel == -1)
            {
                unitRenderer.Render(this, renderLevel);
            }
            else if (renderLevel > unitRenderer.RenderLevel)
            {
                unitRenderer.Render(this, renderLevel);
            }
        }



        //-----------------------------------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// zooms into the unit on the given coordinates until the target type is rendered
        /// <para>returns value at the center of the zoomed point</para>
        /// </summary>
        /// <param name="cameraGeoVector"></param>
        /// <param name="targetZoomLevel"></param>
        /// <returns></returns>
        internal void ZoomTo(GeoVector cameraGeoVector, int targetZoomLevel)
        {
            if (targetZoomLevel > cfg.MaxZoomLevel || targetZoomLevel < 0)
            {
                Debug.Log("Can't zoom any further");
            }
            else if (targetZoomLevel == ZoomLevel)
            {
                //nothing
            }
            else if (targetZoomLevel == ZoomLevel + 1)
            {
                ExpandAround(cameraGeoVector);
            }
            else if (targetZoomLevel > ZoomLevel)
            {
                var idx = GetLocalUnitIndex(cameraGeoVector);
                GetUnit(idx.x, idx.y).ZoomTo(cameraGeoVector, targetZoomLevel);
            }
            else
            {
                //hide children
                //HideRendererDownward();
            }
        }

        /// <summary>
        /// expands the unit on the given coordinates and units around it
        /// <para>returns value at the center</para>
        /// </summary>
        /// <param name="cameraGeoVector"></param>
        /// <returns></returns>
        void ExpandAround(GeoVector cameraGeoVector)
        {
            var center = GetLocalUnitIndex(cameraGeoVector);
            GetUnit(center.x, center.y).Render(5);
            if (ZoomLevel > 1)
                ExpandAroundCoroutine(center);
        }
        void ExpandAroundCoroutine(Vector2Int center)
        {
            for (int i = -PlanetConfig.zoomGenerationRadius; i <= PlanetConfig.zoomGenerationRadius * 2; i++)
            {
                for (int j = -PlanetConfig.zoomGenerationRadius; j <= PlanetConfig.zoomGenerationRadius * 2; j++)
                {
                    GetUnit(center.x + i, center.y + j).Render(3);
                }
            }
        }


        //------------------------------------------------------------Noise Calculation-----------------------------------------------------------------------------------------

    }

}
