using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    Player parent;

    void Start()
    {
        parent = GetComponentInParent<Player>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == parent.gameObject)
            return;
        parent.OnTriggerEnterInChildren(other);
    }
}
