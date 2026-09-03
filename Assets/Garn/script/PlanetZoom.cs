using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlanetZoom : MonoBehaviour
{
    [Header("References")]
    public RectTransform mapContent;
    public RectTransform viewport;
    public ScrollRect scrollRect;

    [Header("Zoom Settings")]
    public float zoomScale = 2.5f;
    public float zoomDuration = 0.5f;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private bool isZoomed = false;
    private bool isAnimating = false;

    void Start()
    {
        originalScale = mapContent.localScale;
        originalPosition = mapContent.position;
    }

    public void ZoomToPlanet()
    {
        if (isAnimating)
            return;

        if (!isZoomed)
        {
            StartCoroutine(ZoomIn());
        }
        else
        {
            StartCoroutine(ZoomOut());
        }
    }

    IEnumerator ZoomIn()
    {
        isAnimating = true;
        isZoomed = true;

        // Disable scrolling while zooming
        scrollRect.enabled = false;

        Vector3 startScale = mapContent.localScale;
        Vector3 targetScale = originalScale * zoomScale;

        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / zoomDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Scale the map
            mapContent.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            // Get the TRUE center of the viewport
            Vector3 viewportCenter = GetViewportCenter();

            // Get the planet's current position
            Vector3 planetCenter = GetPlanetCenter();

            // Move map so planet goes to viewport center
            Vector3 difference = viewportCenter - planetCenter;

            mapContent.position += difference;

            yield return null;
        }

        // Final scale
        mapContent.localScale = targetScale;

        // Final center correction
        Vector3 finalDifference =
            GetViewportCenter() - GetPlanetCenter();

        mapContent.position += finalDifference;

        isAnimating = false;
    }

    IEnumerator ZoomOut()
    {
        isAnimating = true;

        Vector3 startScale = mapContent.localScale;
        Vector3 startPosition = mapContent.position;

        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / zoomDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            mapContent.localScale = Vector3.Lerp(
                startScale,
                originalScale,
                t
            );

            mapContent.position = Vector3.Lerp(
                startPosition,
                originalPosition,
                t
            );

            yield return null;
        }

        mapContent.localScale = originalScale;
        mapContent.position = originalPosition;

        // Enable scrolling again
        scrollRect.enabled = true;

        isZoomed = false;
        isAnimating = false;
    }

    // ==============================
    // GET TRUE VIEWPORT CENTER
    // ==============================

    Vector3 GetViewportCenter()
    {
        Vector3[] corners = new Vector3[4];

        viewport.GetWorldCorners(corners);

        // Bottom-left = corners[0]
        // Top-left    = corners[1]
        // Top-right   = corners[2]
        // Bottom-right = corners[3]

        return (corners[0] + corners[2]) / 2f;
    }

    // ==============================
    // GET PLANET CENTER
    // ==============================

    Vector3 GetPlanetCenter()
    {
        RectTransform planet = GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];

        planet.GetWorldCorners(corners);

        return (corners[0] + corners[2]) / 2f;
    }
}