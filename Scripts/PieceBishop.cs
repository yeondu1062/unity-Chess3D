using UnityEngine;

public class Bishop : ChessPiece
{
    public static GameObject prefab;

    public static Bishop Spawn(float x, float z, int color)
    {
        GameObject instance = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
        Bishop bishop = instance.GetComponent<Bishop>();
        bishop.isWhite = color == 1; bishop.head = 0.85f; return bishop;
    }

    public override bool CanMove(float x, float z)
    {
        float currentX = transform.position.x;
        float currentZ = transform.position.z;

        if (Mathf.Abs(x - currentX) != Mathf.Abs(z - currentZ)) return false;

        float dirX = x > currentX ? 1 : -1;
        float dirZ = z > currentZ ? 1 : -1;

        float checkX = currentX + dirX;
        float checkZ = currentZ + dirZ;

        int maxSteps = 7;
        int steps = 0;

        while (Mathf.Abs(checkX - x) > Mathf.Epsilon && Mathf.Abs(checkZ - z) > Mathf.Epsilon && steps < maxSteps)
        {
            if (ChessManager.GetPieceAtPos(checkX, checkZ) != null) return false;

            checkX += dirX;
            checkZ += dirZ;
            steps++;
        }

        if (steps >= maxSteps) return false;

        ChessPiece targetPiece = ChessManager.GetPieceAtPos(x, z);
        if (targetPiece != null && targetPiece.isWhite == isWhite) return false;

        return true;
    }

    public override void SetMaterial(Material material)
    {
        Renderer renderer = transform.Find("Body").GetComponent<Renderer>();
        defaultMaterial = new Material(renderer.material);
        renderer.material = material;
    }
}
