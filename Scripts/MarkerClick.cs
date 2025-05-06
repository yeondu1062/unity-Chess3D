using UnityEngine;

public class MarkObjectClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        ChessPiece piece = ChessManager.GetPieceAtPos(transform.position.x, transform.position.z);
        Transform selectedPieceT = ChessManager.instance.selectedPiece.transform;

        if (piece != null)
        {
            if (piece.isWhite) ChessManager.instance.aliveWhite--;
            else ChessManager.instance.aliveBlack--;
            Destroy(piece.gameObject);
        }

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
    }
}
