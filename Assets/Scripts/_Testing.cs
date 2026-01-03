using UnityEngine;

public class _Testing : MonoBehaviour
{

    [Header("Player Setup")]
    [SerializeField] private PlayerView playerView;

    [Header("Zones")]
    [SerializeField] private HandZone handZone;
    [SerializeField] private FieldZone fieldZone;

    [Header("Cards")]
    [SerializeField] private CardUnit cardPrefab;
    [SerializeField] private CardUnitSO[] testCards;

    private Player player;

    private void Awake()
    {
        Debug.Log($"Testing Awake, context = {playerView}");
    }

    private void Start()
    {
        Debug.Log($"Testing Start, context = {playerView}");
        player = GameplayController.Instance.Player;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Transform transform = hitInfo.collider.transform;
                Debug.Log($"Clicked on : {transform.name}");
            }
        }
    }

}
