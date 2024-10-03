using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.TestTools;

public class StatisticsEditorTests
{
    [Test]
    public void TestTextUpdate()
    {
        // Create a GameObject and add the TMP_Text component
        var gameObject = new GameObject();
        var textComponent = gameObject.AddComponent<TextMeshProUGUI>();

        // Create the Statistics component and assign the TMP_Text component
        var statistics = gameObject.AddComponent<Statistics>();
        statistics.text = textComponent;

        // Call the Initialize method
        statistics.Initialize();

        // Assert that the text was updated correctly
        Assert.AreEqual("Game statistics here", textComponent.text);
    }

    [Test]
    public void TestTextComponentMissing()
    {
        // Create a GameObject without the TMP_Text component
        var gameObject = new GameObject();

        // Create the Statistics component without assigning the TMP_Text component
        var statistics = gameObject.AddComponent<Statistics>();

        // Capture the log output
        LogAssert.Expect(LogType.Error, "Ei toimi: Teksti-komponentti puuttuu.");

        // Call the Initialize method
        statistics.Initialize();
    }
}


