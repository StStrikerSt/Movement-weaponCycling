using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProfile_", menuName = "Game/Player Profile")]
public class PlayerProfile : ScriptableObject // This is just the movement variables of a given player profile
{
    [Header("Ground Movement")]
    public float walkSpeed = 5f;
    public float groundAcceleration = 20f;
    public float groundDeceleration = 25f;
}
