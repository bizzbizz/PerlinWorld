using UnityEngine;
namespace Assets.Scripts.Common
{
    public class ExceptionManager : MonoBehaviour
    {
        void Awake()
        {
            Application.logMessageReceived += HandleException;
            DontDestroyOnLoad(gameObject);
        }

        void HandleException(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Debug.Log(condition);
                Debug.Log(stackTrace);
            }
            else
            {
                Debug.Log("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                Debug.Log(condition);
                Debug.Log(stackTrace);
            }
        }
    }
}