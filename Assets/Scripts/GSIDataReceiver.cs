using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Handles receiving data from the GSI (Game State Integration) endpoint.
/// This script fetches data from multiple GSI endpoints.
/// </summary>
public class GSIDataReceiver : MonoBehaviour
{
    
    /// <summary>
    /// Stores data retrieved from the statistics endpoint.
    /// </summary>
    public string statisticsData;

    /// <summary>
    /// Stores data retrieved from the player positions endpoint.
    /// </summary>
    public string positionsData; // Stores data from the second address


    /// <summary>
    /// Stores data retrieved from the match data endpoint.
    /// </summary>
    public string matchData;

    /// <summary>
    /// Event triggered when new data is received from the statistics endpoint.
    /// </summary>
    public event Action<string> OnStatisticsDataReceived;
    
    /// <summary>
    /// Event triggered when new data is received from the player positions endpoint.
    /// </summary>
    public event Action<string> OnPositionsDataReceived;

    /// <summary>
    /// Event triggered when new data is received from the match data endpoint.
    /// </summary>
    public event Action<string> OnMatchDataReceived;

    /// <summary>
    /// Unity's Start method, that is called imidiately when script is enabled.
    /// Coroutines to fetch data from multiple endpoints.
    /// </summary>
    private void Start()
    {
        // Start coroutines for each endpoint
        StartCoroutine(GetGSIData("https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/statistics", data => {
            statisticsData = data;
            OnStatisticsDataReceived?.Invoke(statisticsData); // Invoke event for statistics data
        }));

        StartCoroutine(GetGSIData("https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/player_positions", data => {
            positionsData = data;
            OnPositionsDataReceived?.Invoke(positionsData); // Invoke event for positions data
        }));

        StartCoroutine(GetGSIData("https://gsi-ohtuprojekti-staging.apps.ocp-test-0.k8s.it.helsinki.fi/match_data", data => {
            matchData = data;
            OnMatchDataReceived?.Invoke(matchData); // Invoke event for match data
        }));
    }



    /// <summary>
    /// Coroutine to fetch data from a given URI at regular intervals.
    /// </summary>
    /// <param name="uri">The endpoint URI to fetch data from.</param>
    /// <param name="onDataReceived">Callback action to handle the received data.</param>
    /// <returns>An IEnumerator for the coroutine.</returns>
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

            yield return new WaitForSeconds(0.1f); // Wait 0.1 seconds before the next request
        }
    }
}

