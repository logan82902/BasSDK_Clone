namespace ThunderRoad
{
    public class BundleContent : ContainerContent<BundleData, BundleContent>   
    {
        public BundleContent() {}
        public BundleContent(BundleData bundle)
        {
            referenceID = bundle.id;
        }
        public override BundleContent CloneGeneric()
        {
            return new BundleContent
            {
                referenceID = referenceID,
            };
        }
    }
}