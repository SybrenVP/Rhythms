using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BeatView : VisualElement
{
    private Vector3 _startPos = Vector3.zero;
    private Vector3 _endPos = Vector3.zero;

    private float _width;

    public BeatView(Vector3 startPos, Vector3 endPos, float width)
    {
        _startPos = startPos;
        _endPos = endPos;

        _width = width;

        //style.left = _startPos.x;
        //style.top = _startPos.y;
        //style.bottom = _endPos.y;
        //style.right = _endPos.x;
        //
        //style.height = _endPos.y - _startPos.y;
        style.width = _width;
        //style.flexGrow = 1;

        generateVisualContent += OnGenerateVisualContent;
    }

    private void OnGenerateVisualContent(MeshGenerationContext context)
    {
        float width = resolvedStyle.width;
        float height = resolvedStyle.height;
        Vector2 size = new Vector2(width, height);

        Vector2 p0 = _endPos * size;
        Vector2 p1 = _startPos * size;

        Vector2 dir = (p0 - p1).normalized;
        Vector2 ortho = -Vector3.Cross(Vector3.forward, dir);
        Vector2 orthoOffset = ortho * (_width * 0.5f);

        Color color = resolvedStyle.color;

        Vertex[] vertices = new Vertex[4];

        //Bottom left
        vertices[0] = new Vertex
        {
            position = new Vector3 { z = Vertex.nearZ } + (Vector3)(p1 - orthoOffset),
            tint = color,
            uv = Vector2.zero
        };

        //Top left
        vertices[1] = new Vertex
        {
            position = new Vector3 { z = Vertex.nearZ } + (Vector3)(p1 + orthoOffset),
            tint = color,
            uv = new Vector2(0, 1)
        };

        //Top right
        vertices[2] = new Vertex
        {
            position = new Vector3 { z = Vertex.nearZ } + (Vector3)(p0 + orthoOffset),
            tint = color,
            uv = Vector2.one
        };

        //Bottom right
        vertices[3] = new Vertex
        {
            position = new Vector3 { z = Vertex.nearZ } + (Vector3)(p0 - orthoOffset),
            tint = color,
            uv = new Vector2(0, 1)
        };

        MeshWriteData mesh = context.Allocate(4, 6);

        //vertices[1].position = _startPos + new Vector3(0, _width * 0.5f, 0);
        //vertices[2].position = _endPos + new Vector3(0, _width * 0.5f, 0);
        //vertices[3].position = _endPos - new Vector3(0, _width * 0.5f, 0);
        //
        //for (int i = 0; i < vertices.Length; i++)
        //{
        //    vertices[i].position += Vector3.forward * Vertex.nearZ;
        //    vertices[i].tint = Color.white;
        //}

        mesh.SetAllVertices(vertices);
        mesh.SetAllIndices(new ushort[] { 0, 1, 2, 0, 2, 3 });
    }
}
