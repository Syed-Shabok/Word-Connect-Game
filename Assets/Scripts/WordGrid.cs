using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordGrid : MonoBehaviour
{   
    public List<GameObject> letterBoxes = new List<GameObject>();
    public List<TextMeshProUGUI> gridLetters = new List<TextMeshProUGUI>();
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void SetGridLetters(List<string> letterList)
    {
        for(int i = 0; i < letterList.Count; ++i)
        {
            //gridLetters[i].text = letterList[i].ToString();
            gridLetters[i].text = letterList[i];
        }
    }

    //Shakes the wordGrid
    public void ClearGridLetters()
    {
        for(int i = 0; i < gridLetters.Count; ++i)
        {
            gridLetters[i].text = "";
        }
    }

    public void TriggerShake()
    {
        if(animator != null)
        {
            animator.SetTrigger("Shake");
        }
    }
}
