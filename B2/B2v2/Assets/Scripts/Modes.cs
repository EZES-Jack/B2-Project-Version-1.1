using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Modes : MonoBehaviour
{
    public static float CurrentEnemyRange = 20f;
    public static float CurrentVariantRange = 25f;

    [System.Serializable]
    public struct DetectionRangeConfig
    {
        public Button button;
        public float enemyRange;
        public float variantRange;
    }

    [SerializeField] private List<DetectionRangeConfig> modeConfigs;

    private void Awake()
    {
        foreach (var config in modeConfigs)
        {
            if (config.button != null)
            {
                float enemyRange = config.enemyRange;
                float variantRange = config.variantRange;
                config.button.onClick.AddListener(() => SetDetectionRanges(enemyRange, variantRange));
            }
        }
    }

    private void SetDetectionRanges(float enemyRange, float variantRange)
    {
        CurrentEnemyRange = enemyRange;
        CurrentVariantRange = variantRange;
    }
}