using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Statistics : MonoBehaviour
{

    public TMP_Text text; //textMeshPro komponentti!
    public GSIDataReceiver gsiDataReceiver; //GSIDataReceiver scriptistä komponentti
    void Start()
    {
        if (text == null)
        {
            Debug.Log("Teksti komponentissa ongelma"); 
        }
        
        gsiDataReceiver = FindObjectOfType<GSIDataReceiver>();
        if (gsiDataReceiver == null)
        {
            Debug.LogError("Ei toimi: GSI datan haussa ongelma.");
        }
    }

    void Update()
    {
        if (gsiDataReceiver != null && text != null)
        {
            text.text = gsiDataReceiver.gsiData; //päivittää serveriltä saatua dataa näytölle
        }

    }
}