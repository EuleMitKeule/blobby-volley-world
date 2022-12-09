namespace BlobbyVolleyWorld.Entities.Physics.Jumping
{
    public interface IJumpStrategy
    {
        void OnJumpStart();

        void OnJumpHold();

        void OnJumpOver();
    }
}