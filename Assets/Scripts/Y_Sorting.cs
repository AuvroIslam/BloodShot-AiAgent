using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Y_Sorting : MonoBehaviour
{
    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        _sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
}
