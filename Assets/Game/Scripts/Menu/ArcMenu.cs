using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ArcMenu : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Tabs")]
    [SerializeField] private RectTransform[] tabs;

    [Header("Arc")]
    [SerializeField] private float radius = 500f;
    [SerializeField] private float arcAngle = 120f;
    
    [Header("Movement")]
    [SerializeField] private float dragSensitivity = 0.2f;
    [SerializeField] private float smoothSpeed = 8f;

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.cyan;

    [Header("Scale")]
    [SerializeField] private float normalScale = 0.8f;
    [SerializeField] private float selectedScale = 1.2f;

    private float currentRotation;
    private float targetRotation;

    private Vector2 lastPointerPosition;
    
    private int selectedIndex;

    private float angleStep;

    void Start()
    {
        angleStep = arcAngle / 4f;
    }

    void Update()
    {
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * smoothSpeed);
        UpdateTabs();
    }

    void UpdateTabs()
    {
        float bestX = -99999f;
        int bestIndex = 0;

        for (int i = 0; i < tabs.Length; i++)
        {
            // Rotation
            float angle = (-arcAngle * 0.5f) + (i * angleStep) + currentRotation;

            // Boucle
            while (angle < -arcAngle * 0.5f)
            {
                angle += arcAngle + angleStep;
            }

            while (angle > arcAngle * 0.5f)
            {
                angle -= arcAngle + angleStep;
            }

            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Cos(rad) * radius;
            float y = Mathf.Sin(rad) * radius;

            tabs[i].anchoredPosition = new Vector2(x, y);

            //horizontal
            tabs[i].rotation = Quaternion.identity;

            // Détection du plus à droite
            if (x > bestX)
            {
                bestX = x;
                bestIndex = i;
            }
        }

        selectedIndex = bestIndex;

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            bool selected = i == selectedIndex;

            Color targetColor = selected ? selectedColor : normalColor;

            float targetScale = selected ? selectedScale : normalScale;

            // SCALE
            tabs[i].localScale = Vector3.Lerp(tabs[i].localScale, Vector3.one * targetScale,Time.deltaTime * 10f);

            // IMAGE COLOR
            Image image = tabs[i].GetComponent<Image>();

            if (image != null)
            {
                image.color = Color.Lerp(image.color, targetColor,Time.deltaTime * 10f);
            }

            // BUTTON INTERACTION
            Button button = tabs[i].GetComponent<Button>();

            if (button != null)
            {
                button.interactable = selected;
            }
        }
    }

    public void OnBeginDrag(
        PointerEventData eventData)
    {
        lastPointerPosition = eventData.position;
    }

    public void OnDrag(
        PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastPointerPosition;

        targetRotation += delta.y * dragSensitivity;

        lastPointerPosition = eventData.position;
    }

    public void OnEndDrag(
        PointerEventData eventData)
    {
        targetRotation = Mathf.Round(targetRotation / angleStep) * angleStep;
    }
}