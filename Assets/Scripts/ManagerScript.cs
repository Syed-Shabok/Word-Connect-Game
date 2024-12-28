using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;


public class ManagerScript : MonoBehaviour
{   
    public static string wordAttempt = "";
    public GameObject wordAttemptText;
    //public TextMeshProUGUI wordAttemptText;
    public TextMeshProUGUI popupText;
    public GameObject playerControls;
    public static List<string> currentLetterList = new List<string>();

    //Line Renderer Variables.
    public LineRenderer line;
    public LayerMask targetLayerMask;
    private Camera mainCamera;
    private bool isDrawing;
    public List<GameObject> connectedObjects = new List<GameObject>();
    public List<Vector3> drawPositions = new List<Vector3>();
    public TextMeshProUGUI LevelCompleteText;
    public TextMeshProUGUI currentLevelText;

    // Store original colors to revert them later
    private Dictionary<GameObject, Color> originalColors = new Dictionary<GameObject, Color>();
    
    //Daynamic testing variables
    public LevelDataScriptable levelDataScriptable;
    public List<LetterWheel> letterWheels = new List<LetterWheel>(); 
    public List<WordGrid> wordGrids = new List<WordGrid>();
    private List<string> wordsFound = new List<string>();
    private int currentLevel = 14;
    private int solvedWords = 0;
    private Coroutine displayAnswerCoroutine; // Reference to the running DisplayAnswer coroutine

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        SetUpLevel();
    }

    // Update is called once per frame
    void Update()
    {   
        GetPlayerInput();
        ShowPlayerInput();
    }

    //Sets up the UI and at the begining of each level.
    private void SetUpLevel()
    {   
        //Removes all game UI when all levels are complete
        if(currentLevel == levelDataScriptable.levels.Count)
        {
           LevelCompleteText.text = "Game Complete!";
           playerControls.SetActive(false);
           
           for(int i = 0; i < wordGrids.Count; ++i)
           {
                wordGrids[i].gameObject.SetActive(false);
           }
        }
        else
        {   
            //Clears letter boxes at the beginig of each level.
            for(int i = 0; i < wordGrids.Count; ++i)
            {
                wordGrids[i].ClearGridLetters();
            }

            //Removes all words from the previous level. 
            wordsFound.Clear();

            //Shows the current level on screen.
            currentLevelText.text = "Level: " + (currentLevel + 1).ToString();

            //List to store the current level's input letters (i.e. letter wheel letters). 
            List<string> tempList = new List<string>();
            
            //Inserting required letters for each level into tempList.
            int noOfCrrentLevelletters = levelDataScriptable.levels[currentLevel].letters.Count;
            for(int i = 0; i < noOfCrrentLevelletters; ++i)
            {
                tempList.Add((levelDataScriptable.levels[currentLevel].letters[i]).ToString()); 
            }

            //Setting the required Letter-Wheel for current level.
            LetterWheel currentWheel = SetWheel();

            //Adding the required letters from tempList to the Letter-Wheel.
            currentWheel.SetWheelLetters(tempList);

            //Activates all Word Grids and thier corresponding Letter Boxes. 
            for(int i = 0; i < wordGrids.Count; ++i)
            {
                wordGrids[i].gameObject.SetActive(true);
                int numberOfLetterBoxes = wordGrids[i].letterBoxes.Count;

                for(int j = 0; j < numberOfLetterBoxes; ++j)
                {
                    wordGrids[i].letterBoxes[j].SetActive(true);
                }
            }

            //Finds and stores the number of Guess Words that are in the current level.
            int numberOfGuessWords = levelDataScriptable.levels[currentLevel].guessWords.Count;

            //Deactivates Word Grids that are not needed for the current level.
            for(int i = numberOfGuessWords; i < wordGrids.Count; ++i)
            {
                wordGrids[i].gameObject.SetActive(false);
            }

            //Goes into each activated Word Grid and deactivates corresponding unnecessary Letter Boxes. 
            for(int i = 0; i < numberOfGuessWords; ++i)
            {
                int wordLength = levelDataScriptable.levels[currentLevel].guessWords[i].Length;

                for(int j = wordLength; j < wordGrids[i].letterBoxes.Count; ++j)
                {
                    wordGrids[i].letterBoxes[j].SetActive(false);
                }
            }
        }
    }

    //Gets the letters that are swiped over by players and generates lines connecting each letter.    
    private void GetPlayerInput()
    {
        //Assigning Letter highlight color.
        Color highlightColor;
        if (!ColorUtility.TryParseHtmlString("#FFDD62", out highlightColor))
        {
            Debug.LogError("Invalid color code!");
        }
            
        if (Input.GetMouseButtonDown(0))
        {   
            //If players starts inputing another answer before the previous answer is still being
            //displayed this if condition removes the the previous answer.  
            if (displayAnswerCoroutine != null)
            {   
                wordAttempt = "";
                wordAttemptText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                StopCoroutine(displayAnswerCoroutine);
            }

            Vector2 mousePosition = GetMouseWorldInputPosition();
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f, targetLayerMask);
            
            if (hit.collider != null)
            {   
                GameObject targetObject = hit.collider.gameObject;
                GameObject targetLetter = targetObject.transform.GetChild(0).gameObject;

                wordAttempt += targetLetter.GetComponent<TextMeshPro>().text;
                currentLetterList.Add(targetLetter.GetComponent<TextMeshPro>().text);

                isDrawing = true;
                connectedObjects.Add(targetObject);

                //Change the targetObjects Color
                ChangeSpriteColor(targetObject, highlightColor);

                line.gameObject.SetActive(true);
                DrawLine();
            }
        }

        if (Input.GetMouseButton(0) && isDrawing)
        {   
            Vector2 mousePosition = GetMouseWorldInputPosition();
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f, targetLayerMask);
            
            if (hit.collider != null)
            {
                GameObject targetObject = hit.collider.gameObject;
                GameObject targetLetter = targetObject.transform.GetChild(0).gameObject;

                if (!connectedObjects.Contains(targetObject))
                {
                    connectedObjects.Add(targetObject);

                    //Change the targetObjects Color
                    ChangeSpriteColor(targetObject, highlightColor);

                    wordAttempt += targetLetter.GetComponent<TextMeshPro>().text;
                    currentLetterList.Add(targetLetter.GetComponent<TextMeshPro>().text);
                }
            }
            
            DrawLine();
        }

        if (Input.GetMouseButtonUp(0) && isDrawing)
        {
            isDrawing = false;

            //After player lets go of mouse button revert the colors of the target objects
            foreach (GameObject targetObject in connectedObjects)
            {
                RevertSpriteColor(targetObject);
            }   

            connectedObjects.Clear();
            DeActivateDrawing();
            CheckPlayerInput();
        }
    }

    //Checks to see if word provided by player is corrct. 
    private void CheckPlayerInput()
    {   
        int numberOfGuessWords = levelDataScriptable.levels[currentLevel].guessWords.Count;

        for(int i = 0; i < numberOfGuessWords; ++i)
        {   
            if(wordAttempt == levelDataScriptable.levels[currentLevel].guessWords[i])
            {   
                //Checks if player has already found the inputted word.
                //Reduce Complexity.
                if(wordsFound.Contains(wordAttempt))
                {   
                    popupText.color = Color.blue;
                    ShowPlayerAnswer(wordAttempt, true);
                    ShowPopup("Already found!");
                    
                    //Makes the wordGrid containing the inputted word shake. 
                    wordGrids[i].TriggerShake();
                }
                else
                {
                    Debug.Log("Correct Answer.");
                    ShowPlayerAnswer(wordAttempt, true);
                    popupText.color = Color.blue;
                    ShowPopup("Correct!");

                    wordsFound.Add(wordAttempt);
                    ++solvedWords;

                    //Sets the letters from the discovered word to corrosponding Word Grid.
                    wordGrids[i].SetGridLetters(currentLetterList);
                }

                break;
            }
            
            //Reduce Complexity. 
            else if(levelDataScriptable.levels[currentLevel].bonusWords.Contains(wordAttempt))
            {
                popupText.color = Color.blue;
                ShowPlayerAnswer(wordAttempt, true);
                ShowPopup("Bonus Word!");
            }

            else
            {
                Debug.Log("Wrong Answer.");
                ShowPlayerAnswer(wordAttempt, false);
                popupText.color = Color.red;
                ShowPopup("Wrong!");       
            }
        }


        //wordAttempt = "";
        currentLetterList.Clear();
        
        Debug.Log($"Number of solved: {solvedWords}");
        if(solvedWords == numberOfGuessWords)
        {
            ++currentLevel;
            solvedWords = 0;
            ShowPopup("Level Complete!");
            Invoke("SetUpLevel", 1.0f);
        }
    }

    //Shows the combined letters selected by players. 
    private void ShowPlayerInput()
    {   
        
        wordAttemptText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = wordAttempt;

        //wordAttemptText.SetText("<mark=#F4E1180>" + wordAttempt + "</mark>");
        //wordAttemptText.SetText("<mark=#F4E11840>" + wordAttempt + "</mark>");
    }


    //Shows pop-up message to tell players if they are right/wrong.
    public void ShowPopup(string message)
    {
        StartCoroutine(DisplayPopup(message, 1f));
    }

    //Removes pop-up message after a given amount of time.
    private IEnumerator DisplayPopup(string message, float duration)
    {
        popupText.text = message;

        yield return new WaitForSeconds(duration);

        popupText.text = "";
    }

    //Shows the players given answer highlighted in blue if correct or red if wrong.
    private void ShowPlayerAnswer(string answer, bool correct)
    {
        if (displayAnswerCoroutine != null)
        {
            StopCoroutine(displayAnswerCoroutine);
        }
        
        displayAnswerCoroutine = StartCoroutine(DisplayAnswer(answer, correct, 0.5f));
    }

    //Removes the players given answer after a given amount of time.
    private IEnumerator DisplayAnswer(string answer, bool correct, float duration)
    {   
        string a = answer;

        if(correct)
        {   
            Color correctColor;
            if (ColorUtility.TryParseHtmlString("#2E58FF", out correctColor))
            {
                wordAttemptText.GetComponent<Image>().color = correctColor;
            }
        }
        else
        {
            Color wrongColor;
            if (ColorUtility.TryParseHtmlString("#FF4F2E", out wrongColor))
            {
                wordAttemptText.GetComponent<Image>().color = wrongColor;
            }
        }

        wordAttemptText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = wordAttempt;

        yield return new WaitForSeconds(duration);

        Color newColor;
        if (ColorUtility.TryParseHtmlString("#F5FF98", out newColor))
        {
            wordAttemptText.GetComponent<Image>().color = newColor;
        }

        wordAttemptText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        wordAttempt = "";
    }

    //Get mouse position in world space
    public Vector2 GetMouseWorldInputPosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    //Draw the line connecting objects
    public void DrawLine()
    {
        drawPositions.Clear();

        foreach (var targetObject in connectedObjects)
        {
            drawPositions.Add(targetObject.transform.position);
            drawPositions.Add(targetObject.transform.position);
            drawPositions.Add(targetObject.transform.position);
            drawPositions.Add(targetObject.transform.position);
        }

        Vector2 inputDrawPosition = GetMouseWorldInputPosition();
        drawPositions.Add(inputDrawPosition);

        line.positionCount = drawPositions.Count;
        line.SetPositions(drawPositions.ToArray());
    }

    //Deactivate the line drawing
    public void DeActivateDrawing()
    {
        line.positionCount = 0;
        drawPositions.Clear();
        line.gameObject.SetActive(false);
    }

    //Activates and returns the required Letter-Wheel based on current level.
    private LetterWheel SetWheel()
    {
        LetterWheel wheel;
        
        for(int i = 0; i < letterWheels.Count; ++i)
            {
                letterWheels[i].gameObject.SetActive(true);
            }
            
            if(currentLevel < 15)
            {   
                wheel = letterWheels[0];

                letterWheels[1].gameObject.SetActive(false);
                letterWheels[2].gameObject.SetActive(false);
            }
            else if(currentLevel < 29)
            {   
                wheel = letterWheels[1];

                letterWheels[0].gameObject.SetActive(false);
                letterWheels[2].gameObject.SetActive(false);
            }
            else
            {   
                wheel = letterWheels[2];

                letterWheels[0].gameObject.SetActive(false);
                letterWheels[1].gameObject.SetActive(false);     
            }

            return wheel;
    }

    // Change the color of letter cicrle's SpriteRenderer
    private void ChangeSpriteColor(GameObject targetObject, Color newColor)
    {
        SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Store the original color if not already stored
            if (!originalColors.ContainsKey(targetObject))
            {
                originalColors[targetObject] = spriteRenderer.color;
            }

            // Change the color
            spriteRenderer.color = newColor;
        }
    }

    // Revert the letter circle's SpriteRenderer's color to its original
    private void RevertSpriteColor(GameObject targetObject)
    {
        if (originalColors.ContainsKey(targetObject))
        {
            SpriteRenderer spriteRenderer = targetObject.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // Revert the color
                spriteRenderer.color = originalColors[targetObject];
            }

            // Remove from the dictionary to free memory
            originalColors.Remove(targetObject);
        }
    }
}

[System.Serializable]
public class LevelData
{
    public List<string> guessWords = new List<string>();
    public List<string> bonusWords = new List<string>();
    public List<char> letters = new List<char>();
}
