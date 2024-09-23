using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GSIDataReceiver : MonoBehaviour
{
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
                    // Printtaa GSI Datan Unity terminaaliin
                    Debug.Log("GSI Data: " + webRequest.downloadHandler.text);
                }
            }

            yield return new WaitForSeconds(0.1f); // Odotus
        }
    }
}
