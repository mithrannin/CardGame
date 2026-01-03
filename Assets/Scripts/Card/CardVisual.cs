using UnityEngine;

public class CardVisual : MonoBehaviour
{
    [SerializeField] private float hoverLift = 1f;
    [SerializeField] private float hoverAdjustUp = 1f;
    [SerializeField] private float hoverScale = 1.2f;
    [SerializeField] private float animationSpeed = 10f;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform visualRoot;

    private Vector3 baseLocalPos;
    private Vector3 baseScale;

    private bool isHovered;
    private bool isSelected;
    private bool isGreyedOut;

    private void Awake()
    {
        baseLocalPos = transform.localPosition;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        Vector3 targetPos = baseLocalPos + (isHovered ? new Vector3(0f, 1f * hoverLift, 1f * hoverAdjustUp) : Vector3.zero);
        Vector3 targetScale = baseScale * (isHovered ? hoverScale : 1f);

        visualRoot.localPosition = Vector3.Lerp(visualRoot.localPosition, targetPos, Time.deltaTime * animationSpeed);
        visualRoot.localScale = Vector3.Lerp(visualRoot.localScale, targetScale, Time.deltaTime * animationSpeed);

        canvasGroup.alpha = isGreyedOut ? 0.5f : 1f;
    }

    public void SetHovered(bool hovered)
    {
        isHovered = hovered;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
    }

    public void SetGreyedOut(bool greyedOut)
    {
        isGreyedOut = greyedOut;
    }

    public void SetRaycastBlocking(bool value)
    {
        canvasGroup.blocksRaycasts = value;
    }

    public void ResetBase()
    {
        baseLocalPos = Vector3.zero;
        baseScale = Vector3.one;
    }

    public void SetBaseTransform(Vector3 localPos, Vector3 scale)
    {
        baseLocalPos = localPos;
        baseScale = scale;
    }

}

