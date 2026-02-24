using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

namespace ThunderRoad
{
	public static class Yielders
    {
        private static readonly Dictionary<float, WaitForSeconds> _timeInterval = new(100, new FloatComparer());
        private static readonly Dictionary<float, WaitForSecondsRealtime> _realTimeInterval = new(100, new FloatComparer());

        private static readonly WaitForEndOfFrame _endOfFrame = new();

        public static WaitForEndOfFrame EndOfFrame => _endOfFrame;

        private static readonly WaitForFixedUpdate _fixedUpdate = new();

        public static WaitForFixedUpdate FixedUpdate => _fixedUpdate;

        public static WaitForSeconds ForSeconds(float seconds)
        {
            WaitForSeconds wfs;
            if (!_timeInterval.TryGetValue(seconds, out wfs))
            {
                wfs = new WaitForSeconds(seconds);
                _timeInterval.Add(seconds, wfs);
            }
            return wfs;
        }

        public static WaitForSecondsRealtime ForRealSeconds(float seconds)
        {
            return new WaitForSecondsRealtime(seconds);
            // Leaving this code here for the future; We need to figure out a better way to "hand out" WaitForSecondsRealtime instances without giving out the same instance to multiple coroutines
            /*
            WaitForSecondsRealtime wfsr;
            if (!_realTimeInterval.TryGetValue(seconds, out wfsr))
                _realTimeInterval.Add(seconds, wfsr = new WaitForSecondsRealtime(seconds));
            return wfsr;
            */
        }
        
        
    }
}
