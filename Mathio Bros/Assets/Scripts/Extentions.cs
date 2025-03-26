using UnityEngine;

public static class Extentions
{
    public static bool Raycast(this Rigidbody2D rb, Vector2 direction)
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            return false;
        }

        float radius = 0.25f;
        float distance = 0.375f;

        Physics2D.CircleCast(rb.position, radius, direction, distance);
    }
}
