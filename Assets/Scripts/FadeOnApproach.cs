using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to a trigger collider (set as "Is Trigger") placed near the object
// that should fade. Fades the target object's material alpha to 0 once the
// player enters range, then disables it. One-shot - does not fade back in.
public class FadeOnApproach : MonoBehaviour
{
    [Tooltip("Just drag in the object you want to fade - knight, painting, mask, etc.")]
    public GameObject targetObject;

    [Tooltip("How long the fade takes, in seconds.")]
    public float fadeDuration = 3f;

    [Tooltip("Disable the object's collider/renderer once fully faded.")]
    public bool disableAfterFade = true;

    private bool hasTriggered = false;
    private List<Material> instancedMaterials = new List<Material>();

    private void Awake()
    {
        if (targetObject == null) return;

        // Grab every renderer under this object (covers multi-part models)
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer rend in renderers)
        {
            // .material (not .sharedMaterial) creates a unique instance per
            // renderer, so fading this object never affects others sharing
            // the same source material asset.
            instancedMaterials.Add(rend.material);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        if (instancedMaterials.Count == 0) yield break;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);

            foreach (Material mat in instancedMaterials)
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }

            yield return null;
        }

        foreach (Material mat in instancedMaterials)
        {
            Color c = mat.color;
            c.a = 0f;
            mat.color = c;
        }

        if (disableAfterFade)
        {
            targetObject.SetActive(false);
        }
    }

    [ContextMenu("Test Fade")]
    private void TestFade()
    {
        if (!hasTriggered)
        {
            hasTriggered = true;
            StartCoroutine(FadeOut());
        }
    }

    private void OnDestroy()
    {
        // Clean up instanced materials to prevent memory leaks
        foreach (Material mat in instancedMaterials)
        {
            Destroy(mat);
        }
    }
}