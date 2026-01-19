using UnityEngine;
using DG.Tweening;
using TMPro;

public class CookingController : MonoBehaviour
{
    [Header("Temperature")]
    public float temperature = 0f;
    public float minTemp = 0f;
    public float maxTemp = 100f;
    public float perfectMin = 45f;
    public float perfectMax = 55f;
    public float burnThreshold = 90f;

    [Header("Speed")]
    public float heatSpeed = 30f;
    public float coolSpeed = 20f;

    [Header("Cooking")]
    public float timeToCook = 3f;
    public bool validated = false;
    [HideInInspector] public float cookTimer = 0f;
    [HideInInspector] public bool cooked = false;
    [HideInInspector] public bool burned = false;

    [Header("Visual")]
    public SpriteRenderer hotnessIndicator;
    public SpriteRenderer flameSprite;
    public SpriteRenderer secondaryFlameSprite;
    public Color coldColor = Color.blue;
    public Color warmColor = Color.yellow;
    public Color perfectColor = new Color(1f, 0.5f, 0f);
    public Color burnedColor = Color.black;

    [Header("Flame")]
    public GameObject flameBone;
    public Vector3 flameMinScale = Vector3.zero;
    public Vector3 flameMaxScale = Vector3.one * 1.2f;
    public float pulseDuration = 0.5f;
    public float pulseScale = 1.1f;

    [Header("Thermometer")]
    public Transform mask;
    public float maxMaskScaleY = 0.95f;
    public Transform temperatureIndicator;
    public float indicatorMinY = 0f;
    public float indicatorMaxY = 1f;
    public Transform perfectIndicatorPrefab;

    [Header("UI")]
    public TMP_Text temperatureText;

    [Space(20)] public bool ignoreVictoryFreeze = false;
    
    private Transform perfectMinIndicator;
    private Transform perfectMaxIndicator;
    private VictoryChecker victoryChecker;
    private Color currentTargetColor;
    private Tween flamePulseTween;

    void Awake()
    {
        Transform t = transform;
        while (t != null && victoryChecker == null)
        {
            victoryChecker = t.GetComponent<VictoryChecker>();
            t = t.parent;
        }

        if (victoryChecker == null)
            Debug.LogWarning($"CookingController : Aucun VictoryChecker trouvé pour {name} dans les parents.");
    }

    void Start()
    {
        if (perfectIndicatorPrefab != null)
        {
            perfectMinIndicator = Instantiate(perfectIndicatorPrefab, mask.parent);
            perfectMaxIndicator = Instantiate(perfectIndicatorPrefab, mask.parent);

            perfectMinIndicator.localPosition = GetIndicatorPosition(perfectMin);
            perfectMaxIndicator.localPosition = GetIndicatorPosition(perfectMax);
        }
        
        if (flameBone != null)
        {
            flamePulseTween = flameBone.transform.DOPunchScale(Vector3.one * pulseScale, pulseDuration, 1, 0.3f)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.InOutSine)
                .Pause();
        }
    }
    
    Vector3 GetIndicatorPosition(float temp)
    {
        float normalized = Mathf.InverseLerp(minTemp, maxTemp, temp);
        Vector3 pos = temperatureIndicator != null ? temperatureIndicator.localPosition : Vector3.zero;
        pos.y = Mathf.Lerp(indicatorMinY, indicatorMaxY, normalized);
        return pos;
    }

    void Update()
    {
        bool heating = Input.GetMouseButton(0);

        temperature += (heating ? heatSpeed : -coolSpeed) * Time.deltaTime;
        temperature = Mathf.Clamp(temperature, minTemp, maxTemp);

        float normalizedTemp = Mathf.InverseLerp(minTemp, maxTemp, temperature);

        HandleCookingLogic();
        UpdateColor(normalizedTemp);
        UpdateFlame(normalizedTemp);
        UpdateThermometer(normalizedTemp);

        if (temperatureText != null)
            temperatureText.text = $"{Mathf.RoundToInt(temperature)}°C";
    }

    void HandleCookingLogic()
    {
        if (validated) return;

        if (!burned)
        {
            if (temperature >= perfectMin && temperature <= perfectMax)
            {
                cookTimer += Time.deltaTime;
                if (cookTimer >= timeToCook)
                {
                    cooked = true;
                    validated = true;
                    victoryChecker?.CheckCooked();
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
                victoryChecker?.CheckCooked();
    
                hotnessIndicator.color = burnedColor;
                flameSprite.color = burnedColor;
                if (secondaryFlameSprite != null)
                {
                    Color c = Color.Lerp(burnedColor, Color.white, 0.5f);
                    c.a = 1f;
                    secondaryFlameSprite.color = c;
                }

                Debug.Log("Plat brûlé !");
            }

        }
    }

    void UpdateColor(float normalized)
    {
        if (validated && !burned) return;
        
        Color targetColor;

        if (burned) targetColor = burnedColor;
        else if (temperature >= perfectMin && temperature <= perfectMax) targetColor = perfectColor;
        else if (temperature < perfectMin)
        {
            float t = Mathf.InverseLerp(minTemp, perfectMin, temperature);
            targetColor = Color.Lerp(coldColor, warmColor, t);
        }
        else
        {
            float t = Mathf.InverseLerp(perfectMin, maxTemp, temperature);
            targetColor = Color.Lerp(warmColor, perfectColor, t);
        }

        if (targetColor != currentTargetColor)
        {
            hotnessIndicator.DOKill();
            flameSprite.DOKill();
            secondaryFlameSprite?.DOKill();

            if (hotnessIndicator != null)
            {
                Color hotnessColor = Color.Lerp(targetColor, Color.white, 0.5f);
                hotnessColor.a = Mathf.Lerp(0f, 1f, normalized);
                hotnessIndicator.DOColor(hotnessColor, 0.3f).SetEase(Ease.Linear);
            }
            if (flameSprite != null)
            {
                Color flameColor = Color.Lerp(targetColor, Color.white, 0.5f);
                flameColor.a = Mathf.Lerp(0.2f, 1f, normalized);
                flameSprite.DOColor(flameColor, 0.3f).SetEase(Ease.Linear);
            }
            if (secondaryFlameSprite != null)
            {
                Color secondaryColor = Color.Lerp(targetColor, Color.white, 0.5f);
                secondaryColor.a = Mathf.Lerp(0.3f, 1f, normalized);
                secondaryFlameSprite.DOColor(secondaryColor, 0.3f).SetEase(Ease.Linear);
            }

            currentTargetColor = targetColor;
        }
    }

    void UpdateFlame(float normalized)
    {
        if (flameBone == null) return;

        Vector3 targetScale = Vector3.Lerp(flameMinScale, flameMaxScale, normalized);
        flameBone.transform.localScale = targetScale;

        if (normalized > 0.5f)
            flamePulseTween?.Play();
        else
            flamePulseTween?.Pause();
    }

    void UpdateThermometer(float normalized)
    {
        if (mask != null)
        {
            Vector3 targetScale = mask.localScale;
            targetScale.y = normalized * maxMaskScaleY;
            mask.localScale = targetScale;
        }

        if (temperatureIndicator != null)
        {
            Vector3 indicatorPos = temperatureIndicator.localPosition;
            indicatorPos.y = Mathf.Lerp(indicatorMinY, indicatorMaxY, normalized);
            temperatureIndicator.localPosition = indicatorPos;
        }
    }
}
