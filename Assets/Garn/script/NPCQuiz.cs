using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCQuiz : MonoBehaviour
{
    [Header("Quiz UI")]
    public GameObject quizCanvas;

    [Header("Question")]
    [TextArea(2, 5)]
    public string question = "What is 1 + 1?";

    [Header("Choices")]
    public string choiceA = "1";
    public string choiceB = "2";
    public string choiceC = "3";

    [Header("Correct Answer")]
    [Tooltip("1 = A, 2 = B, 3 = C")]
    [Range(1, 3)]
    public int correctAnswer = 2;

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

    [Header("Door")]
    public GameObject door;

    private bool playerNearby = false;
    private bool answeredCorrectly = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        // Hide quiz
        if (quizCanvas != null)
        {
            quizCanvas.SetActive(false);
        }

        // Set normal map
        if (mapBackground != null && normalMap != null)
        {
            mapBackground.sprite = normalMap;
        }

        // Hide door until quiz is completed
        if (door != null)
        {
            door.SetActive(false);
        }

        // Connect buttons
        if (answerAButton != null)
        {
            answerAButton.onClick.AddListener(() => CheckAnswer(1));
        }

        if (answerBButton != null)
        {
            answerBButton.onClick.AddListener(() => CheckAnswer(2));
        }

        if (answerCButton != null)
        {
            answerCButton.onClick.AddListener(() => CheckAnswer(3));
        }
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        // Open quiz
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenQuiz();
        }

        // Close quiz
        if (quizCanvas != null &&
            quizCanvas.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            CloseQuiz();
        }
    }


    // =========================================================
    // OPEN QUIZ
    // =========================================================

    private void OpenQuiz()
    {
        // Don't open again after completing it
        if (answeredCorrectly)
        {
            return;
        }

        if (quizCanvas == null)
        {
            Debug.LogError("Quiz Canvas is not assigned!");
            return;
        }

        quizCanvas.SetActive(true);


        // Set question
        if (questionText != null)
        {
            questionText.text = question;
        }


        // Set choices
        if (answerAText != null)
        {
            answerAText.text = "A. " + choiceA;
        }

        if (answerBText != null)
        {
            answerBText.text = "B. " + choiceB;
        }

        if (answerCText != null)
        {
            answerCText.text = "C. " + choiceC;
        }


        // Clear result
        if (resultText != null)
        {
            resultText.text = "";
        }


        // Enable buttons
        if (answerAButton != null)
        {
            answerAButton.interactable = true;
        }

        if (answerBButton != null)
        {
            answerBButton.interactable = true;
        }

        if (answerCButton != null)
        {
            answerCButton.interactable = true;
        }
    }


    // =========================================================
    // CHECK ANSWER
    // =========================================================

    private void CheckAnswer(int answer)
    {
        // Correct answer
        if (answer == correctAnswer)
        {
            if (resultText != null)
            {
                resultText.text = "Correct!";
            }

            answeredCorrectly = true;


            // Change background
            ChangeMap();


            // Show door
            ShowDoor();


            // Disable buttons
            if (answerAButton != null)
            {
                answerAButton.interactable = false;
            }

            if (answerBButton != null)
            {
                answerBButton.interactable = false;
            }

            if (answerCButton != null)
            {
                answerCButton.interactable = false;
            }


            Debug.Log("Quiz completed! Door appeared.");
        }
        else
        {
            if (resultText != null)
            {
                resultText.text = "Wrong!";
            }

            Debug.Log("Wrong answer!");
        }
    }


    // =========================================================
    // CHANGE MAP
    // =========================================================

    private void ChangeMap()
    {
        if (mapBackground != null && openDoorMap != null)
        {
            mapBackground.sprite = openDoorMap;

            Debug.Log("Map changed to open door!");
        }
        else
        {
            Debug.LogWarning(
                "Map Background or Open Door Map is not assigned!"
            );
        }
    }


    // =========================================================
    // SHOW DOOR
    // =========================================================

    private void ShowDoor()
    {
        if (door != null)
        {
            door.SetActive(true);

            Debug.Log("Door appeared!");
        }
        else
        {
            Debug.LogWarning(
                "Door GameObject is not assigned!"
            );
        }
    }


    // =========================================================
    // CLOSE QUIZ
    // =========================================================

    private void CloseQuiz()
    {
        if (quizCanvas != null)
        {
            quizCanvas.SetActive(false);
        }
    }


    // =========================================================
    // PLAYER DETECTION
    // =========================================================

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            Debug.Log(
                "Player is near the NPC. Press E."
            );
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