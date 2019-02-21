using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SlicedSpriteMesh : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private AnchoredUI anchoredUI;
    [Tooltip("Only 'Simple' and 'Sliced' are supported yet")]
    public Sprite sourceImage;
    public Image.Type imageType;
    public Vector4 spriteBorders;

    // Use this for initialization
    void Start()
    {
        if (imageType != Image.Type.Simple && imageType != Image.Type.Sliced)
        {
            imageType = Image.Type.Simple;
        }
        if (sourceImage != null)
        {
            spriteBorders = sourceImage.border;
        }
        else
        {
            spriteBorders = Vector4.zero;
        }

        if (anchoredUI.shouldBeDeformed)
        {
            //deformMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void deformMesh()
    {
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        RectTransform rectTrans = ((RectTransform)anchoredUI.transform);
        float anchPosY = rectTrans.anchoredPosition.y;
        float sizeDeltaY = rectTrans.sizeDelta.y;

        Vector3 initValues = anchoredUI.calculateSlicedSpriteInitValues();
        float lowerX = initValues.x;
        float higherX = initValues.y;
        float distance = initValues.z;

        //Project vertices on cylinder
        if (imageType == Image.Type.Sliced)
        {
            Vector2[] coordsUV = mesh.uv;
            for (int i = 0; i < vertices.Length; i++)
            {
                //coordsUV[i] = recalculateCordinates(vertices[i]); // TODO

                float circlePos = lowerX + coordsUV[i].x * (higherX - lowerX);
                vertices[i].x = Mathf.Sin(circlePos) * distance;
                vertices[i].y = (anchPosY - sizeDeltaY / 2) + coordsUV[i].y * sizeDeltaY;
                vertices[i].z = Mathf.Cos(circlePos) * distance;
            }
        }
        else
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                float circlePos = lowerX + vertices[i].x * (higherX - lowerX);
                vertices[i].x = Mathf.Sin(circlePos) * distance;
                vertices[i].y = (anchPosY - sizeDeltaY / 2) + vertices[i].y * sizeDeltaY;
                vertices[i].z = Mathf.Cos(circlePos) * distance;
            }
        }

        mesh.vertices = vertices;
    }

    private Vector2 recalculateCordinates(Vector3 input)
    {
        //LTRB XYZW
        Vector2 newValues = new Vector2();
        if (input.x == 0 || input.x == 1)
        {
            newValues.x = input.x;
        }
        else if (input.x < spriteBorders.x) {

        }
        return newValues;
    }
}
