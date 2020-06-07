using Assets.Scripts.World;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class MinimapSelector : MonoBehaviour
//, UnityEngine.EventSystems.IPointerDownHandler
{
    public PlanetGenerator generator;
    public RectTransform current;
    RectTransform rect;

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    Debug.Log("LocalCursor:...");
    //    Vector2 localCursor;
    //    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out localCursor))
    //        return;
    //    localCursor = new Vector2(200 + localCursor.x, 100 - localCursor.y);
    //    Debug.Log("LocalCursor:" + localCursor);
    //}
    //void OnMouseDown()
    //{
    //    Vector2 localCursor;
    //    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Input.mousePosition, Camera.current, out localCursor))
    //        return;
    //    localCursor = new Vector2(200 + localCursor.x, 100 - localCursor.y);
    //    Debug.Log("OnMouseDown:" + localCursor);
    //}
    //private void OnGUI()
    //{
    //    Debug.Log("OnGUI:" + Event.current.type);
    //    if (Event.current.type == EventType.MouseUp)
    //    {
    //        Vector2 localCursor;
    //        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), Event.current.mousePosition, Camera.current, out localCursor))
    //            return;
    //        localCursor = new Vector2(200 + localCursor.x, 100 - localCursor.y);
    //        Debug.Log("LocalCursor:" + localCursor);
    //    }
    //}
    void OnGUI()
    {
        //use this one:


        //if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint)
        //{
        //    UnityEditor.EditorUtility.SetDirty(this); // this is important, if omitted, "Mouse down" will not be display
        //}
        //else if (Event.current.type == EventType.MouseDown)
        //{
        //    if (rect == null)
        //        rect = GetComponent<RectTransform>();

        //    Vector2 localCursor;
        //    if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Event.current.mousePosition, null, out localCursor))
        //        return;

        //    //calibrate selected point
        //    localCursor = new Vector2(200 + localCursor.x, 5 + (localCursor.y - 300) * .9f);

        //    //calibrate minimap selector
        //    current.anchoredPosition = new Vector3(localCursor.x, -localCursor.y);

        //    generator.Zoom((int)(localCursor.x * 1024 / 200), (int)(localCursor.y * 1024 / 100));

        //    //Debug.Log("OnGUI:" + localCursor);
        //}
        //else if(Event.current.type == EventType.ScrollWheel)
        //{
        //    //Event.current.delta 
        //}
    }
}
