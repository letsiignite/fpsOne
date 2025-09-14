
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
    using UnityEngine.Localization;
    using UnityEngine.Localization.Settings;
    using UnityEngine.Localization.Tables;
using static UnityEngine.Rendering.DebugUI;

public class LocalizationTest : MonoBehaviour
{
    

    async Task Start()
    {
       LocalizedString localizedGreeting = new LocalizedString { TableReference = "StringTableOne", TableEntryReference = "welcome" };
        // Asynchronous access (recommended for performance)
        localizedGreeting.GetLocalizedStringAsync().Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                Debug.Log(" ** Localized Greeting: " + op.Result);
            }
        };
        getLocalString("StringTableOne", "welcome");
        await FetchLocalization();
    }

    public string getLocalString(string table, string key)
    {
        string localizedString = "";
       
        var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key);
        if (stringOperation.IsDone)
        {
            Debug.Log(" Got string = "+ stringOperation.Result);
            localizedString = stringOperation.Result;
        }

       return localizedString;
    }

    public void SetText(string table, string key, TMP_Text textDisplay)
    {
        textDisplay.text = getLocalString( table, key);
    }

    async Task FetchLocalization()
    {
        LocalizationSettings.InitializationOperation.WaitForCompletion();
        var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedString("StringTableOne", "welcome");
       
            Debug.Log(" Got string = " + stringOperation);
            
    }
    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(Locale newLocale)
    {
        // Call your custom method here
        Debug.Log($"Language changed to: {newLocale.Identifier.CultureInfo.EnglishName}");
        getLocalString("StringTableOne", "welcome");
    }

   
}