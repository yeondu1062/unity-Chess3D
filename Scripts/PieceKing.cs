using UnityEngine;

public class King : ChessPiece
{
    public static GameObject prefab;

    public static King Spawn(float x, float z, int color)
    {
        GameObject instance = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
        King king = instance.GetComponent<King>();
        king.isWhite = color == 1; return king;
    }

    public override bool CanMove(float x, float z)
    {
        Vector3 currentPos = transform.position;
        float dx = Mathf.Abs(x - currentPos.x);
        float dz = Mathf.Abs(z - currentPos.z);

        bool isOneStepMove = dx <= 1 && dz <= 1;
        if (!isOneStepMove) return false;

        ChessPiece targetPiece = ChessManager.GetPieceAtPos(x, z);
        return targetPiece == null || targetPiece.isWhite != isWhite;
    }

    public override void SetMaterial(Material material)
    {
        Renderer renderer = transform.Find("Body").GetComponent<Renderer>();
        defaultMaterial = new Material(renderer.material);
        transform.Find("Body_").GetComponent<Renderer>().material = material;
        renderer.material = material;
    }
}
