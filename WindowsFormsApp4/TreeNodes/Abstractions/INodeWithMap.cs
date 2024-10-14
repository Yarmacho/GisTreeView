namespace WindowsFormsApp4.TreeNodes.Abstractions
{
    public interface INodeWithMap
    {
        Initializers.Map Map { get; }

        void SetMap(Initializers.Map map);
    }
}
