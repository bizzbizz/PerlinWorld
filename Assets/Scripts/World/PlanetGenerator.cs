using Assets.Scripts.Common.GeoStructs;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.World
{
    public class PlanetGenerator : MonoBehaviour
    {
        private static PlanetGenerator Instance;

        //------------------------------------------------------------------PROPS-----------------------------------------------------------------------------------

        [Header("-----------< Prefabs >-----------")]
        public UnitRenderer PlanetTerrainRendererPrefab;

        internal Unit root;//the root node of noise gen

        //------------------------------------------------------------------REND-----------------------------------------------------------------------------------

        private System.Collections.Generic.List<UnitRenderer> _renderers;

        public static UnitRenderer CreateRenderer()
        {
            var renderer = GameObject.Instantiate(Instance.PlanetTerrainRendererPrefab);
            //renderer.RenderLevel = -1;
            renderer.transform.parent = Instance.transform;
            renderer.OnRendered += _renderer_OnRendered;
            if (Instance._renderers == null)
                Instance._renderers = new System.Collections.Generic.List<UnitRenderer>();
            Instance._renderers.Add(renderer);
            return renderer;
        }

        private static void _renderer_OnRendered()
        {
            //DestroyObject(_oldRenderer);
        }

        //---------------------------------------------------------------METHODS--------------------------------------------------------------------------------------
        /// <summary>
        /// Main
        /// </summary>
        public void StartPlanetCoroutine(PlanetConfig cfg, Image minimap)
        {
            StartCoroutine(PlanetCoroutine(cfg, minimap));
        }
        private System.Collections.IEnumerator PlanetCoroutine(PlanetConfig cfg, Image minimap)
        {
            Instance = this;
            Cleanup();
            yield return new WaitForSeconds(.5f);
            root = Unit.CreatePlanetUnit(cfg);
            yield return new WaitForSeconds(.5f);
            root.Render(7);
            yield return new WaitForSeconds(4f);
            //moon = Unit.CreateRenderedPlanet(cfg2);
            ResetMinimap(minimap);
            root.ZoomTo(new GeoVector(Mathf.PI, Mathf.PI / 2), 0);
        }

        void Cleanup()
        {
            //remove junk first!
            var children = GetComponentsInChildren<UnitRenderer>(true);
            foreach (var child in children)
            {
                DestroyImmediate(child.gameObject);
            }

        }

        internal void ResetMinimap(Image minimapImage)
        {
            //var cfg = root.cfg;

            ////minimap texture
            //if (!root.IsRendered)
            //    root.Render(UnitRenderer.RenderMode.Good);
            //Texture2D texture = root.Renderer.noiseTex;
            //Sprite sprite = Sprite.Create(texture, new Rect(0, cfg.polarHeight, cfg.PlanetPointSize, cfg.PlanetPointSize - cfg.polarHeight * 2), Vector2.zero);
            //minimapImage.sprite = sprite;
            //texture.Apply();
        }
    }
}
