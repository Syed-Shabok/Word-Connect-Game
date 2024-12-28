using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterWheel : MonoBehaviour
{
    public List<TextMeshPro> wheelLetter = new List<TextMeshPro>();

    //Assignes required letters into the Letter-Wheel.
    public void SetWheelLetters(List<string> letters)
    {
        ShuffleList(letters);

        for(int i = 0; i < wheelLetter.Count; ++i)
        {
            wheelLetter[i].text = letters[i];
        }
    }

    //Shiffles elements in a List of strings. 
    private void ShuffleList(List<string> lst)
    {
        System.Random rng = new System.Random(); 

        for (int i = lst.Count - 1; i > 0; --i)
        {
            int randomIndex = rng.Next(0, i + 1);
            string temp = lst[i];
            lst[i] = lst[randomIndex];
            lst[randomIndex] = temp;
        }
    }
}
