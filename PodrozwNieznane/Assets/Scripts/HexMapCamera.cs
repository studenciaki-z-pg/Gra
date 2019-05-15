using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HexMapCamera : MonoBehaviour
{
    float rotationAngle;
    public float rotationSpeed;
    public HexGrid grid;
    public float moveSpeedMinZoom, moveSpeedMaxZoom;
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

        float rotationDelta = Input.GetAxis("Rotation");
        if (rotationDelta != 0f)
        {
            AdjustRotation(rotationDelta);
        }
    }
    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
    Vector3 ClampPosition(Vector3 position) //ograniczenia
    {
        float xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f)*(2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax =(grid.chunkCountZ * HexMetrics.chunkSizeZ - 1) *(1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }
    void AdjustPosition(float xDelta, float zDelta) //zmiana pozycji
    {
        Vector3 direction =transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;
        Vector3 position = transform.localPosition;
        position += direction * distance;
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
