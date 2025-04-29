/* ____ _                   _____ ____  
  / ___| |__   ___  ___ ___|___ /|  _ \ 
 | |   | '_ \ / _ \/ __/ __| |_ \| | | |
 | |___| | | |  __/\__ \__ \___) | |_| |
  \____|_| |_|\___||___/___/____/|____/ 
    written by @yeondu1062.
*/

using UnityEngine;
using System.Collections.Generic;

public class ChessManager : MonoBehaviour
{
    public static ChessManager instance;

    public Material whiteMaterial;
    public Material selectMaterial;
    public GameObject threatPrefab;
    public GameObject bishopPrefab;
    public GameObject knightPrefab;
    public GameObject queenPrefab;
    public GameObject rookPrefab;
    public GameObject kingPrefab;
    public GameObject pawnPrefab;
    public GameObject markPrefab;

    public ChessPiece selectedPiece;
    public List<GameObject> markers = new List<GameObject>();
    public List<GameObject> threats = new List<GameObject>();

    public int aliveWhite = 16;
    public int aliveBlack = 16;
    public int trun = 0;

    public static ChessPiece GetPieceAtPos(float x, float z)
    {
        foreach (ChessPiece piece in FindObjectsByType<ChessPiece>(FindObjectsSortMode.None))
        {
            if (Vector3.Distance(piece.transform.position, new Vector3(x, 0, z)) < 0.5f) return piece;
        }
        return null;
    }

    public void trunChange()
    {
        FindFirstObjectByType<CameraDrag>().x += 180;
        FindFirstObjectByType<TrunTextUi>().Next(++trun);
        FindFirstObjectByType<ScoreTextUi>().TextUpdate();

        if (selectedPiece.isWhite) Camera.main.backgroundColor = new Color(176f/255, 176f/255, 176f/255);
        else Camera.main.backgroundColor = new Color(245f/255, 230f/255, 204f/255);
    }

    public void SelectedPieceClear()
    {
        if (selectedPiece != null)
        {
            selectedPiece.SetMaterial(selectedPiece.defaultMaterial);
            selectedPiece.isSelect = false;
            selectedPiece = null;

            ClearMarker();
            ClearThreat();
        }
    }

    public void Marker(ChessPiece piece)
    {
        for (float x = 3.5f; x >= -3.5f; x -= 1f)
        {
            for (float z = 3.5f; z >= -3.5f; z -= 1f)
            {
                if (piece.CanMove(x, z))
                {
                    if (GetPieceAtPos(x, z) != null) GetPieceAtPos(x, z).Threat();
                    GameObject marker = Instantiate(markPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    markers.Add(marker);
                }
            }
        }
    }

    private void ClearMarker()
    {
        foreach (var marker in markers) Destroy(marker);
        markers.Clear();
    }

    private void ClearThreat()
    {
        foreach (var threat in threats)
        {
            threat.transform.parent.GetComponent<ChessPiece>().isThreat = false;
            Destroy(threat);
        }
        threats.Clear();
    }

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        Rook.prefab = rookPrefab;
        Bishop.prefab = bishopPrefab;
        Knight.prefab = knightPrefab;
        Queen.prefab = queenPrefab;
        King.prefab = kingPrefab;
        Pawn.prefab = pawnPrefab;

        ChessPiece.threatPrefab = threatPrefab;
        ChessPiece.whitePieceMaterial = whiteMaterial;
        ChessPiece.selectPieceMaterial = selectMaterial;

        King.Spawn(0.5f, 3.5f, 0); King.Spawn(0.5f, -3.5f, 1);
        Queen.Spawn(-0.5f, 3.5f, 0); Queen.Spawn(-0.5f, -3.5f, 1);
        Bishop.Spawn(1.5f, 3.5f, 0); Bishop.Spawn(1.5f, -3.5f, 1); Bishop.Spawn(-1.5f, 3.5f, 0); Bishop.Spawn(-1.5f, -3.5f, 1);
        Knight.Spawn(2.5f, 3.5f, 0); Knight.Spawn(2.5f, -3.5f, 1); Knight.Spawn(-2.5f, 3.5f, 0); Knight.Spawn(-2.5f, -3.5f, 1);
        Rook.Spawn(3.5f, 3.5f, 0); Rook.Spawn(3.5f, -3.5f, 1); Rook.Spawn(-3.5f, 3.5f, 0); Rook.Spawn(-3.5f, -3.5f, 1);
        Pawn.Spawn(3.5f, 2.5f, 0); Pawn.Spawn(3.5f, -2.5f, 1); Pawn.Spawn(-3.5f, 2.5f, 0); Pawn.Spawn(-3.5f, -2.5f, 1);
        Pawn.Spawn(2.5f, 2.5f, 0); Pawn.Spawn(2.5f, -2.5f, 1); Pawn.Spawn(-2.5f, 2.5f, 0); Pawn.Spawn(-2.5f, -2.5f, 1);
        Pawn.Spawn(1.5f, 2.5f, 0); Pawn.Spawn(1.5f, -2.5f, 1); Pawn.Spawn(-1.5f, 2.5f, 0); Pawn.Spawn(-1.5f, -2.5f, 1);
        Pawn.Spawn(0.5f, 2.5f, 0); Pawn.Spawn(0.5f, -2.5f, 1); Pawn.Spawn(-0.5f, 2.5f, 0); Pawn.Spawn(-0.5f, -2.5f, 1);
    }
}
