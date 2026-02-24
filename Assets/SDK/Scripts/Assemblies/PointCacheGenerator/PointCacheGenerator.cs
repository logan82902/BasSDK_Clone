using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ThunderRoad
{
    public static class PointCacheGenerator
    {

        public enum MeshBakeMode
        {
            Vertex,
            Triangle
        }

        public enum Distribution
        {
            Sequential,
            Random,
            RandomUniformArea
        }

        [Flags]
        public enum MapType
        {
            None = 0,
            Position = 1 << 0,
            Normal = 1 << 1,
            All = Position | Normal
        }

    }
}