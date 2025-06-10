using TMPro;
using UI.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    /// <summary>
    /// View component representing a single history entry.
    /// Displays the number and colors the background accordingly.
    /// </summary>
    public class HistoryEntryView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI numberText; // Text component to show the number
        [SerializeField] private Image background; // Background image to set color

        /// <summary>
        /// Sets the display data of this entry from the provided model.
        /// </summary>
        /// <param name="model">The data model containing number and color info.</param>
        public void SetData(HistoryEntryModel model)
        {
            // Update number text
            numberText.text = model.Number.ToString();

            // Set background color based on the color string in the model
            background.color = model.Color switch
            {
                "Red" => Color.red,
                "Green" => Color.green,
                _ => Color.gray // Default color for other or unknown values
            };
        }
    }
}