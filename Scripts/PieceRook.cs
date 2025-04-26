using UnityEngine;

public class Rook : ChessPiece
{
    public static GameObject prefab;

    public static Rook Spawn(float x, float z, int color)
    {
        GameObject instance = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
        Rook rook = instance.GetComponent<Rook>();
        rook.isWhite = color == 1; rook.head = 0.6f; return rook;
    }

    public override bool CanMove(float x, float z)
    {
        Vector3 currentPos = transform.position;

        if (!Mathf.Approximately(x, currentPos.x) && !Mathf.Approximately(z, currentPos.z)) return false;

        Vector2 direction = new Vector2(
            x == currentPos.x ? 0 : Mathf.Sign(x - currentPos.x),
            z == currentPos.z ? 0 : Mathf.Sign(z - currentPos.z)
        );

        Vector2 checkPos = new Vector2(currentPos.x, currentPos.z) + direction;
        const int maxSteps = 6;
        int steps = 1;

        while (!Mathf.Approximately(checkPos.x, x) || !Mathf.Approximately(checkPos.y, z))
        {
            if (steps > maxSteps) return false;
            if (ChessManager.GetPieceAtPos(checkPos.x, checkPos.y) != null) return false;

            checkPos += direction;
            steps++;
        }

        ChessPiece targetPiece = ChessManager.GetPieceAtPos(x, z);
        return targetPiece == null || targetPiece.isWhite != isWhite;
    }

    public override void SetMaterial(Material material)
    {
        Renderer renderer = transform.Find("Body").GetComponent<Renderer>();
        defaultMaterial = new Material(renderer.material);
        renderer.material = material;
    }
}
