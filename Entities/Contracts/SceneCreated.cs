namespace Entities.Contracts
{
    public class SceneCreated
    {
        public int SceneId { get; set; }

        public SceneCreated(int sceneId)
        {
            SceneId = sceneId;
        }
    }
}
