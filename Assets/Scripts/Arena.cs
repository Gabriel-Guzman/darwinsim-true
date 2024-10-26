using UnityEngine;
using UnitySharpNEAT.SharpNEAT.Utility;

public class Arena : MonoBehaviour
{
    private readonly FastRandom _fr = new ();
    private EdgeCollider2D _ec;

    public Vector2 RandomPointWithinBounds()
    {
        return new Vector2((float)_fr.NextDouble() /2, (float)_fr.NextDouble() /2);
    }

    private void Start()
    {
        _ec = GetComponent<EdgeCollider2D>();
    }
}