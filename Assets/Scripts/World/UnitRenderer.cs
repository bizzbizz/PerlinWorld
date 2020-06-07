using UnityEngine;

namespace Assets.Scripts.World
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class UnitRenderer : MonoBehaviour
    {
        public event System.Action OnRendered;

        const int textureQuality = 10;

        /// <summary>
        /// -1 is not rendered. number of points rendered is 2^RenderLevel * 2^RenderLevel
        /// </summary>
        public int RenderLevel { get { return __rendlvlCurrentVal; } private set { __rendlvlCurrentVal = value; } }
        int __rendlvlCurrentVal = -1;

        //keep this until render is finished
        private int _renderLevel = -1;

        private bool _isRendering = false;

        /// <summary>
        /// boundries of the unit to render
        /// </summary>
        protected Unit unit;
        /// <summary>
        /// contains unit data
        /// </summary>
        private Unit dataUnit;
        int xSize;
        int ySize;
        int stepSize;

        internal MeshRenderer meshRenderer { get { if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>(); return _meshRenderer; } }
        MeshRenderer _meshRenderer;

        internal Texture2D noiseTex;
        internal Color[] pix;

        public void Render(Unit unit, int renderLevel)
        {
            if (_isRendering)
            {
                Debug.Log(unit.Surface.Rect.ToString() + " cancelled");
                return;
            }

            this.unit = unit;
            _isRendering = true;
            _renderLevel = renderLevel;

            //must call after set unit
            HideRendererUpward();

            if (unit.CalcLevel + renderLevel > PlanetConfig.PointLog)
                throw new System.Exception("must create more units...");
            dataUnit = unit.GetDataUnit();
            xSize = 1 << _renderLevel;
            ySize = unit.ZoomLevel == 0 ? xSize / 2 : xSize;
            stepSize = PlanetConfig.PointSize / (1 << unit.ZoomLevel - dataUnit.ZoomLevel + renderLevel);

            StartCoroutine(ApplyMeshAndTextureCoroutine());
        }


        #region Show/Hide Renderer
        void ShowRenderer()
        {
            if (!meshRenderer.enabled)
                meshRenderer.enabled = true;
        }
        void HideRenderer()
        {
            if (_isRendering) return;
            if (meshRenderer.enabled)
                meshRenderer.enabled = false;
        }
        internal void HideRendererUpward()
        {
            if (unit.Parent != null && unit.Parent.unitRenderer != null)
            {
                unit.Parent.unitRenderer.HideRenderer();
                unit.Parent.unitRenderer.HideRendererUpward();
            }
        }
        //internal void HideRendererDownward(bool includeSelf = false)
        //{
        //    if (includeSelf && _renderer != null)
        //        HideRenderer();
        //    if (_units != null)
        //        foreach (var item in _units)
        //        {
        //            if (item != null)
        //                item.HideRendererDownward(true);
        //        }
        //}
        #endregion


        private System.Collections.IEnumerator ApplyMeshAndTextureCoroutine()
        {
            Debug.Log(unit.Surface.Rect.ToString() + " ApplyMeshAndTextureCoroutine started");
            yield return null;

            yield return ApplyMeshCoroutine();

            yield return new WaitForSeconds(.5f);

            yield return ApplyTextureCoroutine();

            //done
            RenderLevel = _renderLevel;

            OnRendered?.Invoke();

            _isRendering = false;

            ShowRenderer();
            Debug.Log(unit.Surface.Rect.ToString() + " done");

            yield return null;
        }

        /// <summary>
        /// set the renderer's texture (pixel colors) according to dataUnit
        /// </summary>
        private System.Collections.IEnumerator ApplyTextureCoroutine()
        {
            //init 1D array of pixels
            pix = new Color[xSize * ySize * textureQuality * textureQuality];

            //set pixels
            int ctr = 0;
            for (int y = 0; y < ySize; y++)
                for (int tqy = 0; tqy < textureQuality; tqy++)
                    for (int x = 0; x < xSize; x++)
                        for (int tqx = 0; tqx < textureQuality; tqx++)
                            pix[ctr++] = GetColor(dataUnit.Surface.GetInterpolatedRawNoise(x * stepSize, y * stepSize, (float)tqx / textureQuality, (float)tqy / textureQuality));

            yield return null;

            //set texture
            var rend = GetComponent<Renderer>();
            noiseTex = new Texture2D(xSize * textureQuality, ySize * textureQuality)
            {
                wrapMode = TextureWrapMode.Repeat,
            };
            rend.material = new Material(rend.material)
            {
                mainTexture = noiseTex
            };
            //rend.material.shader = Shader.Find("Diffuse");
            rend.material.SetTextureScale("_MainTex", Vector2.one);
            noiseTex.SetPixels(pix);
            noiseTex.Apply();

            yield return null;
        }

        /// <summary>
        /// create a mesh according to dataUnit
        /// </summary>
        private System.Collections.IEnumerator ApplyMeshCoroutine()
        {
            int length = (xSize + 1) * (ySize + 1);

            Vector3[] verts = new Vector3[length];
            Vector2[] uvs = new Vector2[length];
            int vCtr = 0;

            int[] tris = new int[length * 6];
            int triCtr = 0;

            for (int x = 0; x <= xSize; x++)
            {
                for (int y = 0; y <= ySize; y++)
                {
                    //Add each new vertex in the plane
                    int X = unit.dataStart.x + x * stepSize;
                    int Y = unit.dataStart.y + y * stepSize;
                    verts[vCtr] = dataUnit.Surface.Points[X, Y].FlatOceanVertice.ToVector3();
                    uvs[vCtr] = new Vector2((float)x / xSize, (float)y / ySize);
                    vCtr++;
                }
            }
            //Adds the index of the three vertices in order to make up each of the two tris
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    int tr = (ySize + 1) * (x + 1) + y + 1;
                    int tl = (ySize + 1) * x + y + 1;
                    int bl = (ySize + 1) * x + y;
                    int br = (ySize + 1) * (x + 1) + y;
                    tris[triCtr++] = tr; //Top right
                    tris[triCtr++] = bl; //Bottom left 
                    tris[triCtr++] = br; //Bottom right - First triangle
                    tris[triCtr++] = bl; //Bottom left 
                    tris[triCtr++] = tr; //Top right 
                    tris[triCtr++] = tl; //Top left - Second triangle
                }
            }

            yield return null;

            //Assign verts, uvs, and tris to the mesh
            Mesh procMesh = new Mesh
            {
                name = "planet terrain",
                vertices = verts,
                uv = uvs,
                triangles = tris
            };
            procMesh.RecalculateNormals(); //Determines which way the triangles are facing
            procMesh.RecalculateTangents(); //Determines which way the tangents are facing
            GetComponent<MeshFilter>().mesh = procMesh;//Assign Mesh object to MeshFilter
            GetComponent<MeshCollider>().sharedMesh = procMesh;//Assign Mesh object to MeshFilter

            yield return null;
        }


        #region Coloring
        protected Color GetColor(float value)
        {
            if (value <= deepestLevel)
            {
                //deep ocean
                return new Color(0, 0, value);
            }
            if (value <= oceanLevel)
            {
                //shore sea
                return new Color(
                    .5f - (oceanLevel - value) * 10,
                    .75f - (oceanLevel - value) * 10,
                    1 - (oceanLevel - value) * 10);
            }
            if (value <= oceanLevel + .005f)
            {
                //shore land
                return new Color(1, 1, .5f);
            }
            if (value >= summitLevel)
            {
                //summit
                return Color.white;
            }
            //land
            return new Color(
                .5f + 4 * (oceanLevel - value) / (1 - oceanLevel),
                .5f + 3 * (oceanLevel - value) / (1 - oceanLevel),
                0);
        }

        public static float deepestLevel;
        public static float oceanLevel;
        public static float hillLevel;
        public static float summitLevel;
        #endregion

    }
}
