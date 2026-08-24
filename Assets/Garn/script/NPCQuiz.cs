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

    private bool playerNearby = false;

    void Start()
    {
        quizCanvas.SetActive(false);

        answerAButton.onClick.AddListener(() => CheckAnswer(1));
        answerBButton.onClick.AddListener(() => CheckAnswer(2));
        answerCButton.onClick.AddListener(() => CheckAnswer(3));
    }

    void Update()
    {
        // Press E near NPC
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenQuiz();
        }

        // Press Escape to close
        if (quizCanvas.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseQuiz();
        }
    }

    void OpenQuiz()
    {
        quizCanvas.SetActive(true);

        questionText.text = "What is 1 + 1?";

        answerAText.text = "A. 1";
        answerBText.text = "B. 2";
        answerCText.text = "C. 3";

        resultText.text = "";

        // Enable buttons
        answerAButton.interactable = true;
        answerBButton.interactable = true;
        answerCButton.interactable = true;
    }

    void CheckAnswer(int answer)
    {
        // B is correct
        if (answer == 2)
        {
            resultText.text = "Correct!";
        }
        else
        {
            resultText.text = "Wrong!";
        }

        // Prevent answering again
        answerAButton.interactable = false;
        answerBButton.interactable = false;
        answerCButton.interactable = false;
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