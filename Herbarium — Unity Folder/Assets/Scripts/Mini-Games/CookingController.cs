using UnityEngine;
using DG.Tweening;
using TMPro;

public class CookingController : MonoBehaviour
{
    [Header("🌡 Température")]
    public float temperature = 0f;
    public float minTemp = 0f;
    public float maxTemp = 100f;
    public float perfectMin = 45f;
    public float perfectMax = 55f;
    public float burnThreshold = 90f;

    [Header("⚡ Vitesse")]
    public float heatSpeed = 30f;
    public float coolSpeed = 20f;

    [Header("🍳 Cuisson")]
    public float timeToCook = 3f;
    public bool validated = false;
    [HideInInspector] public float cookTimer = 0f;
    [HideInInspector] public bool cooked = false;
    [HideInInspector] public bool burned = false;

    [Header("🎨 Visuel")]
    public SpriteRenderer spriteRenderer;
    public Color coldColor = Color.blue;
    public Color warmColor = Color.yellow;
    public Color perfectColor = new Color(1f, 0.5f, 0f);
    public Color burnedColor = Color.black;

    [Header("🌡 Thermomètre")]
    public Transform mask;
    public float maxMaskScaleY = 0.95f;

    [Header("🖥 UI")]
    public TMP_Text temperatureText;
    
    // Private
    VictoryChecker victoryChecker;
    Color currentTargetColor;

    void Awake()
    {
        // Cherche le VictoryChecker dans les parents
        Transform t = transform;
        while (t != null && victoryChecker == null)
        {
            victoryChecker = t.GetComponent<VictoryChecker>();
            t = t.parent;
        }

        if (victoryChecker == null)
            Debug.LogWarning($"CookingController : Aucun VictoryChecker trouvé pour {name} dans les parents.");
    }
    
    void Update()
    {
        bool heating = Input.GetMouseButton(0);

        temperature += (heating ? heatSpeed : -coolSpeed) * Time.deltaTime;
        temperature = Mathf.Clamp(temperature, minTemp, maxTemp);

        if (!validated)
        {
            if (!burned)
            {
                if (temperature >= perfectMin && temperature <= perfectMax)
                {
                    cookTimer += Time.deltaTime;
                    if (cookTimer >= timeToCook)
                    {
                        cooked = true;
                        validated = true;
                        victoryChecker.CheckCooked();
                        Debug.Log("Plat parfaitement cuit !");
                    }
                }
                else
                {
                    cookTimer = 0f;
                }

                if (temperature >= burnThreshold)
                {
                    burned = true;
                    validated = true;
                    victoryChecker.CheckCooked();
                    Debug.Log("Plat brûlé !");
                }
            }
        }

        UpdateColor();
        UpdateThermometer();

        if (temperatureText != null)
            temperatureText.text = $"{Mathf.RoundToInt(temperature)}°C";
    }

    void UpdateColor()
    {
        if (validated) return;

        Color targetColor = coldColor;

        if (burned) targetColor = burnedColor;
        else if (cooked) targetColor = perfectColor;
        else if (temperature >= perfectMin) targetColor = perfectColor;
        else if (temperature >= minTemp + (perfectMin - minTemp) / 2) targetColor = warmColor;

        if (targetColor != currentTargetColor)
        {
            spriteRenderer.DOKill();
            spriteRenderer.DOColor(targetColor, 0.3f).SetEase(Ease.Linear);
            currentTargetColor = targetColor;
        }
    }


    void UpdateThermometer()
    {
        if (mask == null) return;

        float normalized = Mathf.Clamp01(temperature / maxTemp);
        Vector3 targetScale = mask.localScale;
        targetScale.y = normalized * maxMaskScaleY;
        mask.localScale = targetScale;
    }
}
