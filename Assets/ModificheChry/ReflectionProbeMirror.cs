using UnityEngine;

public class ReflectionProbeMirror : MonoBehaviour
{
    void Update() => GetComponent<ReflectionProbe>().RenderProbe();
}
