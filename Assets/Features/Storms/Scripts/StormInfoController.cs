using Extensions.Log;
using TMPro;
using UnityEngine;

namespace Blackset.Storms
{
    /// <summary>
    /// Контроллер вывода информации о шторме
    /// </summary>
    public class StormInfoController : MonoBehaviour
    {
        [Header("Активный шторм"), Space]
        [SerializeField]
        private ActiveStorm activeStorm;
        
        [Header("Элементы UI"), Space]
        [SerializeField]
        private GameObject stormPanel;
        [SerializeField]
        private TMP_Text nameText;
        [SerializeField]
        private TMP_Text descriptionText;

        private void OnEnable()
        {
            if (activeStorm == null)
            {
                ServiceDebug.LogError("Хранилище активного шторма не назначено");
                return;
            }
            
            bool stormActive = activeStorm.TryGet(out Storm storm);
            if (!stormActive)
            {
                stormPanel.SetActive(false);
                return;
            }
            
            stormPanel.SetActive(true);
            nameText.text = storm.DataName;
            descriptionText.text = storm.DataDescription;
        }
    }
}