using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;


public class HexMapCamera : MonoBehaviour
{
    public HexGrid grid;
    public float moveSpeed;
    public float swivelMinZoom, swivelMaxZoom;
    public float stickMinZoom, stickMaxZoom;
    float zoom = 1f;
    Transform swivel, stick;
    void Update() 
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }
    Vector3 ClampPosition(Vector3 position) //ograniczenia
    {
        float xMax =
            (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f)*
            (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax =
            (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) *
            (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }
    void AdjustPosition(float xDelta, float zDelta) //zmiana pozycji
    {
        float distance = moveSpeed * Time.deltaTime;
        Vector3 position = transform.localPosition;
        position += new Vector3(xDelta, 0f, zDelta) * distance;
        transform.localPosition = ClampPosition(position);
    }
    void AdjustZoom(float delta) //zmiana przybliżenia
    {
        zoom = Mathf.Clamp01(zoom + delta);
        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }
    void Awake() //referencje do pod zespołów
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }
}
