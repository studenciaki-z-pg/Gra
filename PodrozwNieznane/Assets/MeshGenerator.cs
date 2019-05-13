using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class MeshGenerator : MonoBehaviour
{
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uvs;


    public void GenerateMesh(int[,] map, float tileSize)
    {

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                MeshHex(x, y, tileSize, map[x, y]);
            }
        }


        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    void AssignUVs(int tileType)
    {
        // vertices order: ( determined in function MeshHex() )
        //     5._______.6
        //      /       \
        //   1./    .0   \.2
        //     \         /
        //    3.\_______/.4

        int tileTypesAmount = 2; //adjustable , number of different types
        float translationX = 1f * tileType / tileTypesAmount;
        uvs.Add(new Vector2(translationX + .5f / tileTypesAmount, .5f));
        uvs.Add(new Vector2(translationX + 0f / tileTypesAmount, .5f));
        uvs.Add(new Vector2(translationX + 1f / tileTypesAmount, .5f));

        /*uvs.Add(new Vector2(translationX + .25f / tileTypesAmount, 0));
        uvs.Add(new Vector2(translationX + .75f / tileTypesAmount, 0));
        uvs.Add(new Vector2(translationX + .25f / tileTypesAmount, 1));
        uvs.Add(new Vector2(translationX + .75f / tileTypesAmount, 1));*/ //cut corners of the texture

        uvs.Add(new Vector2(translationX + 0f / tileTypesAmount, 0));
        uvs.Add(new Vector2(translationX + 1f / tileTypesAmount, 0));
        uvs.Add(new Vector2(translationX + 0f / tileTypesAmount, 1));
        uvs.Add(new Vector2(translationX + 1f / tileTypesAmount, 1)); //stretched texture

    }

    void MeshHex(int positionX, int positionY, float tileSize, int tileType) // tileSize = length of flatTop
    {
        int vertexIndex = vertices.Count;
        float hexSideLength = tileSize;
        float hexWideWidth = tileSize * 2; //adjustable
        float hexNarrowWidth = tileSize / 2f + hexWideWidth / 2f;
        float hexHeigth = tileSize * Mathf.Sqrt(3); //adjustable


        float hexPositionX = positionX * hexNarrowWidth; //?
        float hexPositionY = (positionX % 2 == 0) ? (positionY * hexHeigth) : (positionY * hexHeigth + hexHeigth / 2f); //?

        Vector3 middlePoint = new Vector3(hexPositionX, 0, hexPositionY);
        vertices.Add(middlePoint);

        //1
        Vector3 leftPoint = middlePoint + Vector3.left * hexWideWidth / 2f;
        vertices.Add(leftPoint);

        //2
        Vector3 rightPoint = middlePoint + Vector3.right * hexWideWidth / 2f;
        vertices.Add(rightPoint);

        //3
        Vector3 bottomLeftPoint = middlePoint + new Vector3(tileSize / -2f, 0, hexHeigth / -2f);
        vertices.Add(bottomLeftPoint);

        //4
        Vector3 bottomRightPoint = middlePoint + new Vector3(tileSize / 2f, 0, hexHeigth / -2f);
        vertices.Add(bottomRightPoint);

        //5
        Vector3 topLeftPoint = middlePoint + new Vector3(tileSize / -2f, 0, hexHeigth / 2f);
        vertices.Add(topLeftPoint);

        //6
        Vector3 topRightPoint = middlePoint + new Vector3(tileSize / 2f, 0, hexHeigth / 2f);
        vertices.Add(topRightPoint);

        CreateTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 5);
        CreateTriangle(vertexIndex, vertexIndex + 5, vertexIndex + 6);
        CreateTriangle(vertexIndex, vertexIndex + 6, vertexIndex + 2);
        CreateTriangle(vertexIndex, vertexIndex + 2, vertexIndex + 4);
        CreateTriangle(vertexIndex, vertexIndex + 4, vertexIndex + 3);
        CreateTriangle(vertexIndex, vertexIndex + 3, vertexIndex + 1);

        AssignUVs(tileType);
    }


    void CreateTriangle(int a, int b, int c)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }

}