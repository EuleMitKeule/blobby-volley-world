namespace BlobbyVolleyWorld.Match.States
{
    public abstract class MatchStateComponent : StateComponent
    {
        public abstract void OnTimeChanged(object sender, float time);
    }
}