using UnityEngine;
using System.Collections;

public abstract class ChessPiece : MonoBehaviour
{
    public static GameObject threatPrefab;
    public static Material whitePieceMaterial;
    public static Material selectPieceMaterial;
    public Material defaultMaterial;

    private Coroutine threatMotionCoroutine;

    public bool isWhite;
    public bool isSelect;
    public bool isThreat;
    public float head;

    public abstract bool CanMove(float x, float z);
    public abstract void SetMaterial(Material material);

    public void Select()
    {
        isSelect = true;
        SetMaterial(selectPieceMaterial);

        ChessManager.instance.SelectedPieceClear();
        ChessManager.instance.selectedPiece = this;
        ChessManager.instance.Marker(this);
    }

    public void Threat()
    {
        if (threatMotionCoroutine != null) return;
        threatMotionCoroutine = StartCoroutine(ThreatMotionCoroutine());

        GameObject threatInstance = Instantiate(threatPrefab, transform);
        RectTransform threatRect = threatInstance.GetComponent<RectTransform>();
        RectTransform prefabRect = threatPrefab.GetComponent<RectTransform>();

        threatRect.anchoredPosition = prefabRect.anchoredPosition;
        threatRect.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, head, 0);

        ChessManager.instance.threats.Add(threatInstance);
        isThreat = true;
    }

    private IEnumerator ThreatMotionCoroutine()
    {
        Vector3 originalPos = transform.position;

        float duration = 0.3f;
        float magnitude = 0.1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float deltaX = Random.Range(-1, 1) * magnitude;
            float deltaY = Random.Range(-1, 1) * magnitude;

            transform.position = originalPos + new Vector3(deltaX, 0, deltaY);
            yield return null;
        }

        transform.position = originalPos;
        threatMotionCoroutine = null;
    }

    private void OnMouseDown()
    {
        if (isThreat)
        {
            if (isWhite) ChessManager.instance.aliveWhite--;
            else ChessManager.instance.aliveBlack--;

            Transform selectedPieceT = ChessManager.instance.selectedPiece.transform;

            if (ChessManager.instance.playerType == 1)
            {
                NetworkManager.instance.MoveDataToClient(
                    selectedPieceT.position.x, selectedPieceT.position.z,
                    transform.position.x, transform.position.z
                );
            }
            else if (ChessManager.instance.playerType == 2)
            {
                NetworkManager.instance.MoveDataToServer(
                    selectedPieceT.position.x, selectedPieceT.position.z,
                    transform.position.x, transform.position.z
                );
            }

            selectedPieceT.position = transform.position;
            ChessManager.instance.trunChange();
            ChessManager.instance.SelectedPieceClear();

            Destroy(this.gameObject); return;
        }
        if (isWhite != (ChessManager.instance.trun % 2 == 0)) return;
        if (isWhite && ChessManager.instance.playerType == 2) return;
        if (!isWhite && ChessManager.instance.playerType == 1) return;
        if (isSelect) ChessManager.instance.SelectedPieceClear();
        else Select();
    }

    private void Start()
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.size = new Vector3(0.03f, 0.06f, 0.03f);
        box.center = new Vector3(0, 0.03f, 0);

        if(isWhite)
        {
            SetMaterial(whitePieceMaterial);
            transform.Rotate(0, 180, 0);
        }
    }
}
