
using Assets.Scripts.Common.GeoStructs;
using System;
using UnityEngine;

namespace Assets.Scripts.World
{
    public class CameraController : MonoBehaviour
    {
        internal Action<float, float, float> ZoomAction;

        protected PlanetConfig planetCfg;

        /// <summary>
        /// 0-2PI => radians from primary meridian to here (around)
        /// </summary>
        protected float tetha = Mathf.PI;
        /// <summary>
        /// 0-PI => radians from north pole to here (vertical)
        /// </summary>
        protected float phi = Mathf.PI / 2;
        /// <summary>
        /// distance from core
        /// </summary>
        protected float radius;
        /// <summary>
        /// 0-1 => 1 is planet orbit. 0 is a bit above the surface
        /// </summary>
        protected float zoom = 1;

        const int panSensitivity = 2;
        //const int zoomSensitivity = 1000;
        int previousZoomLevel = 0;

        public void SetPlanet(PlanetConfig planetCfg)
        {
            this.planetCfg = planetCfg;

            zoom = 1;
            tetha = Mathf.PI;
            phi = Mathf.PI / 2;
            radius = 2 * (PlanetConfig.PlanetRadius + planetCfg.PlanetSurfaceMaxHeight / 2);
            SetPosition();
        }

        void ScanBelow()
        {
            //this script depends on value of points of units which are perlin based sums. must limit to 1 
            //e.g. 0.5 + 0.25 + ... => 1

            float float_targetZoomLevel = Mathf.Clamp(planetCfg.MaxZoomLevel * (1.1f - zoom), 0, planetCfg.MaxZoomLevel);
            int int_targetZoomLevel = (int)float_targetZoomLevel;
            //float subZoom = Mathf.Clamp(float_targetZoomLevel - float_targetZoomLevel, 0, planetCfg.MaxZoomLevel);

            ZoomAction?.Invoke(tetha, phi, zoom);

            if (Physics.Raycast(transform.position, -transform.position, out RaycastHit hit, PlanetConfig.PlanetRadius))
            {
                radius = hit.point.magnitude * (1 + 1 / Mathf.Pow(PlanetConfig.UnitSize, float_targetZoomLevel));
            }
            else
            {
                radius = PlanetConfig.PlanetRadius * (1 + 1 / Mathf.Pow(PlanetConfig.UnitSize, float_targetZoomLevel));
            }
            SetPosition();

            //switch (int_targetZoomLevel)
            //{
            //    case 0:
            //    case 1:
            //    case 2:
            //    case 3:
            //    case 4:
            //    case 5:
            //    case 6:

            //        if (previousZoomLevel < float_targetZoomLevel)
            //            cameraObject.transform.Rotate(0, 75, -90, Space.Self);
            //        else if (float_targetZoomLevel < previousZoomLevel)
            //            cameraObject.transform.Rotate(0, -75, 90, Space.Self);
            //        break;
            //    case 7:
            //    case 8:
            //    default:
            //        break;
            //}

            previousZoomLevel = int_targetZoomLevel;
        }

        private void Update()
        {
            if (planetCfg == null) return;

            if (Input.GetKey(KeyCode.A))
            {
                tetha -= Time.deltaTime * panSensitivity / zoom;
                SetPosition();
            }
            if (Input.GetKey(KeyCode.D))
            {
                tetha += Time.deltaTime * panSensitivity / zoom;
                SetPosition();
            }
            if (Input.GetKey(KeyCode.W))
            {
                phi -= Time.deltaTime * panSensitivity / zoom;
                SetPosition();
            }
            if (Input.GetKey(KeyCode.S))
            {
                phi += Time.deltaTime * panSensitivity / zoom;
                SetPosition();
            }
            //var wheel = Input.GetAxis("Mouse ScrollWheel") * panSensitivity;

            if (Input.GetKey(KeyCode.Q))
                ScanBelow();

            if (Input.GetMouseButtonDown(0))
            {
                SetPosition();
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, 100))
                {
                    Debug.Log(hit.transform.gameObject.name);
                }
            }
        }
        public void UpdateTetha(float value)
        {
            tetha = value;
            SetPosition();
        }
        public void UpdatePhi(float value)
        {
            phi = value;
            SetPosition();
        }
        public void UpdateZoom(float value)
        {
            zoom = value;
            SetPosition();
        }

        /// <summary>
        /// gets the current position according to tetha, phi and radius
        /// </summary>
        /// <returns></returns>
        private void SetPosition()
        {
            tetha %= Mathf.PI * 2;
            phi %= Mathf.PI;

            //calc spherical positions
            var x = radius * Mathf.Cos(tetha) * Mathf.Sin(phi);
            var z = radius * Mathf.Sin(tetha) * Mathf.Sin(phi);
            var y = radius * Mathf.Cos(phi);

            transform.position = new Vector3(x, y, z);
            transform.LookAt(Vector3.zero);
        }
    }
}
