using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Statistics : MonoBehaviour
{

    public TMP_Text text;

    void Start()
    {
        if (text != null)
        {

            text.text = "Tämä teksti tulee scriptistä";
            Debug.Log("Teksti on päivitetty"); // Log success message
        }
        else
        {
            Debug.LogError("Ei toimi: Teksti-komponentti puuttuu.");
        }
    }


    void Update()
    {
        
    }
}

