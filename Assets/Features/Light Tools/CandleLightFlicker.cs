using UnityEngine;

namespace Features.Light_Tools
{
    /// <summary>
    /// Симуляция мерцаний свечи
    /// </summary>
    [RequireComponent(typeof(Light))]
    public sealed class CandleLightFlicker : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float flickerStrength = 0.2f;
        [SerializeField, Range(0f, 20f)] private float flickerSpeed = 6f;
        [SerializeField, Range(0f, 100f)] private float smoothing = 12f;

        private Light targetLight;
        
        private float baseIntensity;
        private float noiseOffset;
        private float currentIntensity;

        private void Awake()
        {
            targetLight = GetComponent<Light>();

            baseIntensity = targetLight.intensity;
            noiseOffset = Random.Range(0f, 1000f);
            currentIntensity = baseIntensity;
        }

        private void Update()
        {
            float noise = Mathf.PerlinNoise(noiseOffset, Time.time * flickerSpeed);
            float normalizedNoise = (noise - 0.5f) * 2f;
            float targetIntensity = baseIntensity * (1f + normalizedNoise * flickerStrength);

            currentIntensity = Mathf.Lerp(currentIntensity, targetIntensity, Time.deltaTime * smoothing);
            targetLight.intensity = currentIntensity;
        }

        /// <summary>
        /// Обновить базовую интенсивность от текущего значения источника света
        /// </summary>
        public void RefreshBaseIntensity()
        {
            if (targetLight == null) return;

            baseIntensity = targetLight.intensity;
        }
    }
}