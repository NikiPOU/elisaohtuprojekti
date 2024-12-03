using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GSIDataReceiver : MonoBehaviour
{
    public string statisticsData; // Stores data from the first address
    public string positionsData; // Stores data from the second address
    public string matchData;
    public event Action<string> OnStatisticsDataReceived; // Event for the first data source
    public event Action<string> OnPositionsDataReceived; // Event for the second data source
    public event Action<string> OnMatchDataReceived;

    private void Start()
    {
        // Start coroutines for each endpoint
        StartCoroutine(GetGSIData("https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/statistics", data => {
            statisticsData = data;
            OnStatisticsDataReceived?.Invoke(statisticsData); // Invoke event for the first data
        }));

        StartCoroutine(GetGSIData("https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/player_positions", data => {
            positionsData = data;
            OnPositionsDataReceived?.Invoke(positionsData); // Invoke event for the second data
        }));

        StartCoroutine(GetGSIData("https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/match_data", data => {
            matchData = data;
            OnMatchDataReceived?.Invoke(matchData);
        }));
    }

    private IEnumerator GetGSIData(string uri, Action<string> onDataReceived)
    {
        while (true)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                yield return webRequest.SendWebRequest();

                // Handle any errors
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.Log("Error fetching data from " + uri + ": " + webRequest.error);
                }
                else
                {
                    string data = webRequest.downloadHandler.text; // Get the downloaded data
                    Debug.Log($"Data from {uri}: {data}");
                    onDataReceived?.Invoke(data); // Pass the data to the callback
                }
            }

            yield return new WaitForSeconds(0.1f); // Wait before the next request
        }
    }
}

