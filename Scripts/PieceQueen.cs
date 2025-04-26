using UnityEngine;

public class Queen : ChessPiece
{
    public static GameObject prefab;

    public static Queen Spawn(float x, float z, int color)
    {
        GameObject instance = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
        Queen queen = instance.GetComponent<Queen>();
        queen.isWhite = color == 1; queen.head = 0.6f; return queen;
    }

    public override bool CanMove(float x, float z)
    {
        Vector3 currentPos = transform.position;
        float dx = Mathf.Abs(x - currentPos.x);
        float dz = Mathf.Abs(z - currentPos.z);

        bool isStraight = Mathf.Approximately(dx, 0) || Mathf.Approximately(dz, 0);
        bool isDiagonal = Mathf.Approximately(dx, dz);

        if (!isStraight && !isDiagonal) return false;

        Vector2 direction = new Vector2(
            x == currentPos.x ? 0 : Mathf.Sign(x - currentPos.x),
            z == currentPos.z ? 0 : Mathf.Sign(z - currentPos.z)
        );

        Vector2 checkPos = new Vector2(currentPos.x, currentPos.z) + direction;
        const int maxSteps = 7;
        int steps = 1;

        while (!Mathf.Approximately(checkPos.x, x) || !Mathf.Approximately(checkPos.y, z))
        {
            if (ChessManager.GetPieceAtPos(checkPos.x, checkPos.y) != null) return false;

            checkPos += direction;
            if (++steps > maxSteps) return false;
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
