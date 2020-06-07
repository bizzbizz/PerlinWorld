namespace Assets.Scripts.Common.GeoStructs
{
    /// <summary>
    /// XYZ Cartesian Coordinates
    /// </summary>
    [System.Serializable]
    public struct VectorXYZ
    {
        public float x;
        public float y;
        public float z;
        public VectorXYZ(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", x, y, z);
        }
        public static VectorXYZ operator *(VectorXYZ obj, float val)
        {
            return new VectorXYZ(obj.x * val, obj.y * val, obj.z * val);
        }
        public static VectorXYZ operator +(VectorXYZ obj, UnityEngine.Vector3 val)
        {
            return new VectorXYZ(obj.x + val.x, obj.y + val.y, obj.z + val.z);
        }
        public static VectorXYZ operator +(VectorXYZ obj, UnityEngine.Vector3Int val)
        {
            return new VectorXYZ(obj.x + val.x, obj.y + val.y, obj.z + val.z);
        }
        public UnityEngine.Vector3 ToVector3()
        {
            return new UnityEngine.Vector3(x, y, z);
        }
    }
}
