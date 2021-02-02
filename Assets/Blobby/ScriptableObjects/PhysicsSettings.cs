using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Physics Settings", menuName = "", order = 0)]
public class PhysicsSettings : ScriptableObject
{
    [Header("Ball")]
    [Range(5f, 150f)]
    public float ballGravity = 40f;
    [Range(5f, 150f)]
    public float ballMaxVelocity = 40f;
    // [Range(5f, 150f)]
    // public float ballShotVelocity = 35f;
    [Range(1f, 10f)]
    public float ballFriction = 4f;
    [Range(100f, 1500f)]
    public float ballSpin = 900f;

    [Header("Player")]
    [Range(5f, 50f)]
    public float playerRunVelocity = 18f;
    [Range(10f, 150f)]
    public float playerJumpVelocity = 55f;
    [Range(10f, 350f)]
    public float playerJumpDrift = 190f;
    [Range(50f, 500f)]
    public float playerGravity = 340f;
    [Range(0.2f, 1.5f)]
    public float playerMinCharge = 0.85f;
    [Range(1f, 3.5f)]
    public float playerMaxCharge = 1.85f;
    [Range(0.025f, 0.4f)]
    public float playerChargeIncrease = 0.14f;
    [Range(0.01f, 0.1f)]
    public float playerChargeTime = 0.04f;
    
}