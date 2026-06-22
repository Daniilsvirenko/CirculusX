using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnApproach : MonoBehaviour
{
    [Header("Fade Settings")]
    public List<GameObject> objectsToFade = new List<GameObject>();
    public float fadeDuration = 2f;
    public float delayBetweenObjects = 0.5f;
    public bool disableAfterFade = true;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool hasTriggered = false;

    private static readonly int BaseColorProp = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProp = Shader.PropertyToID("_Color");

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        foreach (GameObject target in objectsToFade)
        {
            if (target == null) continue;

            List<Material> instancedMaterials = new List<Material>();
            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();

            foreach (Renderer rend in renderers)
            {
                // CRITICAL FIX: Use .materials (plural) to capture ALL slots on this renderer
                Material[] sharedMats = rend.materials;
                foreach (Material mat in sharedMats)
                {
                    if (mat != null)
                    {
                        instancedMaterials.Add(mat);
                    }
                }
            }

            StartCoroutine(FadeIndividualObject(target, instancedMaterials));
            yield return new WaitForSeconds(delayBetweenObjects);
        }
    }

    private IEnumerator FadeIndividualObject(GameObject obj, List<Material> materials)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float progress = t / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, fadeCurve.Evaluate(progress));

            foreach (Material mat in materials)
            {
                if (mat == null) continue;

                if (mat.HasProperty(BaseColorProp))
                {
                    Color c = mat.GetColor(BaseColorProp);
                    c.a = alpha;
                    mat.SetColor(BaseColorProp, c);
                }
                if (mat.HasProperty(ColorProp))
                {
                    Color c = mat.GetColor(ColorProp);
                    c.a = alpha;
                    mat.SetColor(ColorProp, c);
                }
            }
            yield return null;
        }

        // Clean up transparency and disable object
        if (disableAfterFade && obj != null)
        {
            obj.SetActive(false);
        }

        // Destroy runtime instances to clean up memory leaks safely AFTER disabling
        foreach (Material mat in materials)
        {
            if (mat != null) Destroy(mat);
        }
    }
}