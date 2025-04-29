using UnityEngine;

public class MarkObjectClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        ChessPiece piece = ChessManager.GetPieceAtPos(transform.position.x, transform.position.z);
        if (piece != null)
        {
            if (piece.isWhite) ChessManager.instance.aliveWhite--;
            else ChessManager.instance.aliveBlack--;
            Destroy(piece.gameObject);
        }

        ChessManager.instance.selectedPiece.transform.position = transform.position;
        ChessManager.instance.trunChange();
        ChessManager.instance.SelectedPieceClear();
    }
}
