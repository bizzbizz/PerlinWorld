using System.IO;
using UnityEngine;
using UnityEngine.Networking;
namespace Assets.Scripts.Common.Utils
{
    public static class FileUtils
    {
        public static string RereadFile(string fileName)
        {
            //copies and unpacks file from apk to persistentDataPath where it can be accessed
            string destinationPath = PathUtils.GetLocalPath(fileName);
            string sourcePath = PathUtils.GetInstallerPath(fileName);

            //UnityEngine.Debug.Log(string.Format("{0}-{1}-{2}-{3}", sourcePath,  File.GetLastWriteTimeUtc(sourcePath), File.GetLastWriteTimeUtc(destinationPath)));

            //copy whatsoever

            //if DB does not exist in persistent data folder (folder "Documents" on iOS) or source DB is newer then copy it
            //if (!File.Exists(destinationPath) || (File.GetLastWriteTimeUtc(sourcePath) > File.GetLastWriteTimeUtc(destinationPath)))
            {
                if (sourcePath.Contains("://"))
                {
                    // Android  
                    var www = new UnityWebRequest(sourcePath);
                    while (!www.isDone) {; }                // Wait for download to complete - not pretty at all but easy hack for now 
                    if (string.IsNullOrEmpty(www.error))
                    {
                        File.WriteAllText(destinationPath, www.downloadHandler.text);
                    }
                    else
                    {
                        Debug.Log("ERROR: the file DB named " + fileName + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                    }
                }
                else
                {
                    // Mac, Windows, Iphone                
                    //validate the existens of the DB in the original folder (folder "streamingAssets")
                    if (File.Exists(sourcePath))
                    {
                        //copy file - alle systems except Android
                        File.Copy(sourcePath, destinationPath, true);
                    }
                    else
                    {
                        Debug.Log("ERROR: the file DB named " + fileName + " doesn't exist in the StreamingAssets Folder, please copy it there.");
                    }
                }
            }

            StreamReader reader = new StreamReader(destinationPath);
            var encJsonString = reader.ReadToEnd();
            reader.Close();
            return encJsonString;

            //int num = 1655887995;
            //var jsonString = funera.security.StringCipher.Decrypt(encJsonString, "studiostark" + num);
            //return jsonString;
        }
        public static void OverwriteFile(string data)
        {
            //var file = File.Open("File_Name", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //var writer = new StreamWriter(file);
            File.WriteAllText("C:/Filename.txt", data);
        }
    }
}