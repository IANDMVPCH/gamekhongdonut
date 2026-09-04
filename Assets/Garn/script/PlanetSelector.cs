using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetSelector : MonoBehaviour
{
    [Header("Map")]
    public RectTransform mapContent;
    public RectTransform viewport;
    public ScrollRect scrollRect;

    [Header("Planets")]
    public RectTransform[] planets;

    [Header("Planet Scenes")]
    [Tooltip("Must match the 'planets' array index-for-index. Use the exact scene name as it appears in Build Settings.")]
    public string[] sceneNames;

    [Header("Zoom")]
    public float zoomScale = 2.5f;
    public float zoomDuration = 0.5f;

    [Header("Arrows")]
    public Button leftArrow;
    public Button rightArrow;

    [Header("Enter Planet Buttons")]
    [Tooltip("One button per planet, index-matched to 'planets'. Each button should be set up to call EnterPlanet() with its own index, OR just leave onClick empty and let this script wire it automatically in Start().")]
    public Button[] enterPlanetButtons;

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

        // Hide all enter buttons initially and wire each one to its own index
        if (enterPlanetButtons != null)
        {
            for (int i = 0; i < enterPlanetButtons.Length; i++)
            {
                if (enterPlanetButtons[i] == null)
                    continue;

                enterPlanetButtons[i].gameObject.SetActive(false);

                int capturedIndex = i; // avoid closure bug
                enterPlanetButtons[i].onClick.AddListener(() => EnterPlanet(capturedIndex));
            }
        }
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

        HideAllEnterButtons();

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

            mapContent.localScale = Vector3.Lerp(startScale, targetScale, t);

            Vector3 planetCenter = GetPlanetCenter();
            Vector3 viewportCenter = GetViewportCenter();
            Vector3 difference = viewportCenter - planetCenter;

            mapContent.position = startPosition + difference;

            yield return null;
        }

        mapContent.localScale = targetScale;

        CenterPlanet();

        isMoving = false;

        UpdateArrows();
        ShowEnterButtonForCurrentPlanet();
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

            Vector3 planetCenter = GetPlanetCenter();
            Vector3 viewportCenter = GetViewportCenter();
            Vector3 difference = viewportCenter - planetCenter;

            Vector3 targetPosition = mapContent.position + difference;

            mapContent.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return null;
        }

        CenterPlanet();

        isMoving = false;

        UpdateArrows();
        ShowEnterButtonForCurrentPlanet();
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

        HideAllEnterButtons();

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

        HideAllEnterButtons();

        StartCoroutine(MoveToPlanet());
    }

    // ==========================================
    // ENTER PLANET (LOAD SCENE)
    // ==========================================

    public void EnterPlanet(int index)
    {
        if (index < 0 || sceneNames == null || index >= sceneNames.Length)
            return;

        string sceneName = sceneNames[index];

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"No scene name assigned for planet index {index}.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    // ==========================================
    // ENTER BUTTON VISIBILITY
    // ==========================================

    void HideAllEnterButtons()
    {
        if (enterPlanetButtons == null)
            return;

        foreach (var btn in enterPlanetButtons)
        {
            if (btn != null)
                btn.gameObject.SetActive(false);
        }
    }

    void ShowEnterButtonForCurrentPlanet()
    {
        if (enterPlanetButtons == null || currentPlanet < 0 || currentPlanet >= enterPlanetButtons.Length)
            return;

        HideAllEnterButtons();

        if (enterPlanetButtons[currentPlanet] != null)
            enterPlanetButtons[currentPlanet].gameObject.SetActive(true);
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