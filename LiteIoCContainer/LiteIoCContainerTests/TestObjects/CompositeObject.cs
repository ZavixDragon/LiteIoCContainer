namespace LiteIoCContainerTests.TestObjects
{
    public class CompositeObject
    {
        public ISimple Simple { get; }

        public CompositeObject(ISimple simple)
        {
            Simple = simple;
        }
    }
}
