using System.IO;
using UnityEngine;

namespace Assets.Scripts.Common.Utils
{
    public static class PathUtils
    {
        /// <summary>
        /// Gets a local path in project folder which is writable
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetLocalPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, fileName);
        }

        /// <summary>
        /// Gets a local path in installer folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetInstallerPath(string fileName)
        {
#if UNITY_EDITOR
            return Path.Combine(Application.streamingAssetsPath, fileName);
#else
		return "jar:file://" + Application.dataPath + "!/assets/" + fileName;
#endif
        }
    }
}