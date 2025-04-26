using UnityEngine;

public class Knight : ChessPiece
{
    public static GameObject prefab;

    public static Knight Spawn(float x, float z, int color)
    {
        GameObject instance = Instantiate(prefab, new Vector3(x, 0, z), Quaternion.Euler(0, 90, 0));
        Knight knight = instance.GetComponent<Knight>();
        knight.isWhite = color == 1; knight.head = 0.7f; return knight;
    }

    public override bool CanMove(float x, float z)
    {
        Vector3 currentPos = transform.position;
        float dx = Mathf.Abs(x - currentPos.x);
        float dz = Mathf.Abs(z - currentPos.z);

        bool isLShaped = (dx == 2 && dz == 1) || (dx == 1 && dz == 2);
        if (!isLShaped) return false;

        ChessPiece target = ChessManager.GetPieceAtPos(x, z);
        return target == null || target.isWhite != isWhite;
    }

    public override void SetMaterial(Material material)
    {
        Renderer renderer = transform.Find("Body").GetComponent<Renderer>();
        defaultMaterial = new Material(renderer.material);
        renderer.material = material;
    }
}
