using Assets.Scripts.Common.GeoStructs;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class Game : MonoBehaviour
    {
        public PlanetGenerator Generator;
        public UnityEngine.UI.Image minimap;
        public CameraController cameraController;

        public PlanetConfig planetCfg;

        private void Start()
        {
            CreatePlanet();

            cameraController.SetPlanet(planetCfg);
            //cameraController.ZoomAction = (tetha, phi, zoom) => Generator.root.ZoomTo(new GeoVector(tetha, phi), int_targetZoomLevel);
        }
        public void CreatePlanet()
        {
            planetCfg.InitPlanetConfig();

            Generator.StartPlanetCoroutine(planetCfg, minimap);
        }
    }
}
