namespace BlobbyVolleyWorld.Entities.Input.Jumping
{
    public class NoJumpInputStrategy : IJumpInputStrategy
    {
        InputComponent InputComponent { get; }

        public NoJumpInputStrategy(InputComponent inputComponent)
        {
            InputComponent = inputComponent;
        }

        public void OnJumpDown() { }
        public void OnJumpUp() { }
    }
}