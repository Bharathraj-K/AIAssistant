using UnityEngine;

public class CursorMovement : MonoBehaviour
{
    Vector2 pos;
    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = pos;
    }
}
