using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlanetSelector : MonoBehaviour
{
    [Header("Map")]
    public RectTransform mapContent;
    public RectTransform viewport;
    public ScrollRect scrollRect;

    [Header("Planets")]
    public RectTransform[] planets;

    [Header("Zoom")]
    public float zoomScale = 2.5f;
    public float zoomDuration = 0.5f;

    [Header("Arrows")]
    public Button leftArrow;
    public Button rightArrow;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private int currentPlanet = -1;

    private bool isZoomed = false;
    private bool isMoving = false;

    void Start()
    {
        originalScale = mapContent.localScale;
        originalPosition = mapContent.position;

        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
    }

    // ==========================================
    // CLICK PLANET
    // ==========================================

    public void SelectPlanet(int index)
    {
        if (isMoving)
            return;

        if (index < 0 || index >= planets.Length)
            return;

        currentPlanet = index;

        if (!isZoomed)
        {
            StartCoroutine(ZoomToPlanet());
        }
        else
        {
            StartCoroutine(MoveToPlanet());
        }
    }

    // ==========================================
    // ZOOM INTO PLANET
    // ==========================================

    IEnumerator ZoomToPlanet()
    {
        isMoving = true;
        isZoomed = true;

        scrollRect.enabled = false;

        Vector3 startScale = mapContent.localScale;
        Vector3 targetScale = originalScale * zoomScale;

        Vector3 startPosition = mapContent.position;

        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / zoomDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Zoom
            mapContent.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            // Center planet
            Vector3 planetCenter = GetPlanetCenter();
            Vector3 viewportCenter = GetViewportCenter();

            Vector3 difference = viewportCenter - planetCenter;

            // Calculate target position from original position
            mapContent.position =
                startPosition + difference;

            yield return null;
        }

        mapContent.localScale = targetScale;

        // Final centering
        CenterPlanet();

        isMoving = false;

        UpdateArrows();
    }

    // ==========================================
    // MOVE TO ANOTHER PLANET
    // ==========================================

    IEnumerator MoveToPlanet()
    {
        isMoving = true;

        Vector3 startPosition = mapContent.position;

        float time = 0f;

        while (time < zoomDuration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / zoomDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            // Current planet position
            Vector3 planetCenter = GetPlanetCenter();
            Vector3 viewportCenter = GetViewportCenter();

            Vector3 difference = viewportCenter - planetCenter;

            Vector3 targetPosition =
                mapContent.position + difference;

            mapContent.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            yield return null;
        }

        // Make sure it is perfectly centered
        CenterPlanet();

        isMoving = false;

        UpdateArrows();
    }

    // ==========================================
    // LEFT ARROW
    // ==========================================

    public void PreviousPlanet()
    {
        if (!isZoomed || isMoving)
            return;

        if (currentPlanet <= 0)
            return;

        currentPlanet--;

        StartCoroutine(MoveToPlanet());
    }

    // ==========================================
    // RIGHT ARROW
    // ==========================================

    public void NextPlanet()
    {
        if (!isZoomed || isMoving)
            return;

        if (currentPlanet >= planets.Length - 1)
            return;

        currentPlanet++;

        StartCoroutine(MoveToPlanet());
    }

    // ==========================================
    // CENTER CURRENT PLANET
    // ==========================================

    void CenterPlanet()
    {
        if (currentPlanet < 0)
            return;

        Vector3 planetCenter = GetPlanetCenter();
        Vector3 viewportCenter = GetViewportCenter();

        Vector3 difference = viewportCenter - planetCenter;

        mapContent.position += difference;
    }

    // ==========================================
    // GET PLANET CENTER
    // ==========================================

    Vector3 GetPlanetCenter()
    {
        RectTransform planet = planets[currentPlanet];

        Vector3[] corners = new Vector3[4];

        planet.GetWorldCorners(corners);

        return (corners[0] + corners[2]) / 2f;
    }

    // ==========================================
    // GET VIEWPORT CENTER
    // ==========================================

    Vector3 GetViewportCenter()
    {
        Vector3[] corners = new Vector3[4];

        viewport.GetWorldCorners(corners);

        return (corners[0] + corners[2]) / 2f;
    }

    // ==========================================
    // ARROW VISIBILITY
    // ==========================================

    void UpdateArrows()
    {
        if (currentPlanet > 0)
            leftArrow.gameObject.SetActive(true);
        else
            leftArrow.gameObject.SetActive(false);

        if (currentPlanet < planets.Length - 1)
            rightArrow.gameObject.SetActive(true);
        else
            rightArrow.gameObject.SetActive(false);
    }
}