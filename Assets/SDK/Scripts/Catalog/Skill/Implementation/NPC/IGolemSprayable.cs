using System;


using TriInspector;

namespace ThunderRoad
{
    public interface IGolemSprayable
    {
        public abstract void GolemSprayStart(GolemSpray ability, out Action end);
    }
}
