using UnityEngine;

public class _Testing : MonoBehaviour
{

    [Header("Player Setup")]
    [SerializeField] private PlayerContextView playerView;

    [Header("Zones")]
    [SerializeField] private HandZone handZone;
    [SerializeField] private FieldZone fieldZone;

    [Header("Cards")]
    [SerializeField] private CardUnit cardPrefab;
    [SerializeField] private CardUnitSO[] testCards;

    private PlayerContext player;

    private void Awake()
    {
        Debug.Log($"Testing Awake, context = {playerView.Context}");
    }

    private void Start()
    {
        Debug.Log($"Testing Start, context = {playerView.Context}");
        player = playerView.Context;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnCardToHand();
        }

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

    void SpawnCardToHand()
    {
        CardUnit card = Instantiate(cardPrefab);

        card.Initialize(testCards[Random.Range(0, testCards.Length)]);
        card.SetOwner(player.Controller);
        card.SetPlayable(true); // bypass turn/action checks for testing
        player.Hand.AddCard(card);
        Debug.Log($"Spawned card '{card.name}' for {player.Controller.name}");
    }


}
