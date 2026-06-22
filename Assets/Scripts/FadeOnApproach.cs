using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOnApproach : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("List of transparent objects to fade out in order.")]
    public List<GameObject> objectsToFade = new List<GameObject>();

    [Tooltip("How long each individual object takes to fade out.")]
    public float fadeDuration = 2f;

    [Tooltip("The delay (in seconds) before the NEXT object starts fading.")]
    public float delayBetweenObjects = 0.5f;

    [Tooltip("Disable the object entirely once its fade finishes.")]
    public bool disableAfterFade = true;

    [Tooltip("Controls the speed of the fade. Use an S-curve for maximum smoothness!")]
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool hasTriggered = false;

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
                instancedMaterials.Add(rend.material);
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

            // Calculate progress and evaluate it against our smooth curve
            float progress = t / fadeDuration;
            float alpha = Mathf.Lerp(1f, 0f, fadeCurve.Evaluate(progress));

            foreach (Material mat in materials)
            {
                if (mat == null) continue;
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
            yield return null;
        }

        // Clean up memory leaks from instanced materials
        foreach (Material mat in materials)
        {
            if (mat != null)
            {
                Color c = mat.color;
                c.a = 0f;
                mat.color = c;
                Destroy(mat);
            }
        }

        if (disableAfterFade && obj != null)
        {
            obj.SetActive(false);
        }
    }
}