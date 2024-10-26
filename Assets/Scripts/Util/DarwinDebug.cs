using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public static class DarwinDebug
    {
        private static Dictionary<string, float> LastSentTime = new ();

        public static void Log(string message, float timeInterval = 0.5f)
        {
            float lastTime;
            if (!LastSentTime.TryGetValue(message, out lastTime))
            {
                lastTime = 0;
                LastSentTime.Add(message, Time.time);
            }
            else
            {
                LastSentTime[message] = Time.time;
            }
            
            if (Time.time - lastTime > timeInterval)
                Debug.Log(message);
        }
    }
}