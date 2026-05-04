using TMPro;
using UnityEngine;

namespace Game
{
    [System.Serializable]
    public class Quote
    {
        public string text;
        public string author;
    }

    public class QuoteDisplayOnDeath : MonoBehaviour
    {
        public TextMeshProUGUI quoteText;
        public TextMeshProUGUI authorText;

        public Quote[] quotes;

        private void OnEnable()
        {
            ShowRandomQuote();
        }

        void ShowRandomQuote()
        {
            if (quotes == null || quotes.Length == 0) return;

            int index = Random.Range(0, quotes.Length);
            Quote q = quotes[index];

            quoteText.text = q.text;
            authorText.text = q.author;
        }
    }
}