using UnityEngine;

public class DontRotate : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform _rt;

    private void Start()
    {
        _rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        _rt.transform.rotation = Quaternion.identity;
    }
}
