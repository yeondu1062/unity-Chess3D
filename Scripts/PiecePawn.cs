using UnityEngine;

public class Pawn : ChessPiece
{
    public static GameObject prefab;

    public static Pawn Spawn(float x, float z, int color)
    {
        GameObject instance = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
        Pawn pawn = instance.GetComponent<Pawn>();
        pawn.isWhite = color == 1; pawn.head = 0.46f; return pawn;
    }

    public override bool CanMove(float x, float z)
    {
        Vector3 currentPos = transform.position;
        int dir = isWhite ? 1 : -1;

        if (Mathf.Approximately(currentPos.x, x) && Mathf.Approximately(currentPos.z + dir, z))
        {
            return ChessManager.GetPieceAtPos(x, z) == null;
        }

        if (Mathf.Approximately(currentPos.x, x) && Mathf.Approximately(currentPos.z + dir * 2, z) &&
            Mathf.Approximately(currentPos.z, (!isWhite ? 2.5f : -2.5f)))
        {
            return ChessManager.GetPieceAtPos(x, currentPos.z + dir) == null && ChessManager.GetPieceAtPos(x, z) == null;
        }

        if (Mathf.Abs(x - currentPos.x) == 1 && Mathf.Approximately(currentPos.z + dir, z))
        {
            ChessPiece targetPiece = ChessManager.GetPieceAtPos(x, z);
            return targetPiece != null && targetPiece.isWhite != isWhite;
        }

        return false;
    }

    public override void SetMaterial(Material material)
    {
        Renderer renderer = transform.Find("Body").GetComponent<Renderer>();
        defaultMaterial = new Material(renderer.material);
        renderer.material = material;
    }
}
