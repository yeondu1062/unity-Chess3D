using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public int xSpeed = 800;
    public int ySpeed = 800;
    public int zoomSpeed = 20;

    public int angleMin = -20;
    public int angleMax = 80;
    public int distMin = 2;
    public int distMax = 40;

    public float distance = 10f;
    public float x = -20f;
    public float y = 30f;

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
            y = Mathf.Clamp(y, angleMin, angleMax);
        }

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, distMin, distMax);

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0, 0, -distance) + Vector3.zero;

        transform.rotation = rotation;
        transform.position = position;
    }
}
