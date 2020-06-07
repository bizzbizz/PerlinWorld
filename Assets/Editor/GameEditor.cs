using Assets.Scripts.World;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Game))]
// ^ This is the script we are making a custom editor for.
public class GameEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (Game)target;
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("CREATE C-137"))
        {
            script.CreatePlanet();
        }
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //z1 = System.Convert.ToInt32(GUILayout.TextField(z1.ToString()));
        //z2 = System.Convert.ToInt32(GUILayout.TextField(z2.ToString()));
        //if (GUILayout.Button("MegaTerrain"))
        //{
        //    script.ZoomPlanet(new GeoVector(z1, z2), UnitType.MegaTerrain);
        //}
        //GUILayout.EndHorizontal();
    }

    //double z1 = 5, z2 = 5;
}