using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    private float distance = 3;

    void Start()
    {
        transform.LookAt(target);
    }

    void Update()
    {
        //set transform
        transform.position = target.position - transform.forward * distance;

        //rotation
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = false;

            float angleX = 2 * Input.GetAxis("Mouse X");
            float angleY = 2 * Input.GetAxis("Mouse Y");

            transform.RotateAround(target.position, Vector3.up, angleX);

            if ((angleY > 0 && (transform.eulerAngles.x < 60 || transform.eulerAngles.x > 180))
            || (angleY < 0 && (transform.eulerAngles.x > 320 || transform.eulerAngles.x < 180)))
            {
                transform.RotateAround(target.position, transform.right, angleY);
            }

            transform.LookAt(target);
        }
        else Cursor.visible = true;

        //zoom
        float zoom = 2 * Input.GetAxis("Mouse ScrollWheel");

        if ((distance > 2 && zoom < 0) || (zoom > 0 && distance < 10))
        {
            distance += zoom;
        }
    }
}
