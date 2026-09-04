using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCQuiz : MonoBehaviour
{
    [Header("Quiz UI")]
    public GameObject quizCanvas;

    [Header("Question")]
    public TMP_Text questionText;

    [Header("Answer Buttons")]
    public Button answerAButton;
    public Button answerBButton;
    public Button answerCButton;

    [Header("Answer Text")]
    public TMP_Text answerAText;
    public TMP_Text answerBText;
    public TMP_Text answerCText;

    [Header("Result")]
    public TMP_Text resultText;

    [Header("Map Background")]
    public SpriteRenderer mapBackground;

    public Sprite normalMap;
    public Sprite openDoorMap;

    private bool playerNearby = false;
    private bool answeredCorrectly = false;

    void Start()
    {
        quizCanvas.SetActive(false);

        // Set the original map
        if (mapBackground != null && normalMap != null)
        {
            mapBackground.sprite = normalMap;
        }

        answerAButton.onClick.AddListener(() => CheckAnswer(1));
        answerBButton.onClick.AddListener(() => CheckAnswer(2));
        answerCButton.onClick.AddListener(() => CheckAnswer(3));
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenQuiz();
        }

        if (quizCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseQuiz();
        }
    }

    void OpenQuiz()
    {
        // Don't open the quiz again after answering correctly
        if (answeredCorrectly)
            return;

        quizCanvas.SetActive(true);

        questionText.text = "What is 1 + 1?";

        answerAText.text = "A. 1";
        answerBText.text = "B. 2";
        answerCText.text = "C. 3";

        resultText.text = "";

        answerAButton.interactable = true;
        answerBButton.interactable = true;
        answerCButton.interactable = true;
    }

    void CheckAnswer(int answer)
    {
        if (answer == 2)
        {
            resultText.text = "Correct!";

            answeredCorrectly = true;

            // CHANGE MAP TO OPEN DOOR
            ChangeMap();

            answerAButton.interactable = false;
            answerBButton.interactable = false;
            answerCButton.interactable = false;
        }
        else
        {
            resultText.text = "Wrong!";
        }
    }

    void ChangeMap()
    {
        if (mapBackground != null && openDoorMap != null)
        {
            mapBackground.sprite = openDoorMap;
        }
    }

    void CloseQuiz()
    {
        quizCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            CloseQuiz();
        }
    }
}