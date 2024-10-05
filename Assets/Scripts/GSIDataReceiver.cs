using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GSIDataReceiver : MonoBehaviour
{
    public string gsiData; //UUTTA tähän muuttujaan talletetaan gsi dataa
    public event Action<string> OnDataReceived;
    private void Start()
    {
        StartCoroutine(GetGSIData("http://localhost:3000/data"));   // Kovakoodattuna portti 3000 väliaikaisesti
    }

    private IEnumerator GetGSIData(string uri)
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                // Jos Error -> printtaa sen Unity terminaaliin
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error: " + webRequest.error);
                }
                else
                {
                    gsiData = webRequest.downloadHandler.text; //gsiData muuttujaan talletetaan haettu data
                    // Printtaa GSI Datan Unity terminaaliin
                    Debug.Log("GSI Data: " + webRequest.downloadHandler.text);
                    OnDataReceived?.Invoke(gsiData);
                }
            }

            yield return new WaitForSeconds(0.1f); // Odotus
        }
    }
}
