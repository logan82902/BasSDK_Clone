
using TriInspector;

namespace ThunderRoad
{
    [System.Serializable]
    public class SpellContent : ContainerContent<SpellData, SpellContent>
    {
        public override SpellContent CloneGeneric()
        {
            return new SpellContent(data);
        }

        public SpellContent() { }

        public SpellContent(string referenceID)
        {
            this.referenceID = referenceID;
        }

        public SpellContent(SpellData data)
        {
            this.referenceID = data.id;
        }
    }
}
