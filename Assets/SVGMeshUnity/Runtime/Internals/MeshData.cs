﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVGMeshUnity.Internals
{
    public class MeshData
    {
        public MeshData()
        {
            Vertices = new List<Vector3>();
            Edges = new List<int>();
            Triangles = new List<int>();
            VertexIndices = new Hashtable();
        }
        
        public List<Vector3> Vertices { get; private set; }
        public List<int> Edges { get; private set; }
        public List<int> Triangles { get; private set; }
        
        public int EdgeCount
        {
            get { return Edges.Count / 2; }
        }

        private Hashtable VertexIndices;

        public void Clear()
        {
            Vertices.Clear();
            Edges.Clear();
            Triangles.Clear();
            VertexIndices.Clear();
        }

        public void AddVertices(WorkBuffer<Vector2> buffer)
        {
            var firstEdgeIdx = -1;
            var prevEdgeidx = -1;

            var vertices = Vertices;
            var edges = Edges;
            var indicies = VertexIndices;

            var size = buffer.UsedSize;
            var data = buffer.Data;

            for (var i = 0; i < size; ++i)
            {
                var v = data[i];
                var idx = -1;

                var index = indicies[v];
                if (index != null)
                {
                    idx = (int) index;
                }
                
                if (idx == -1)
                {
                    vertices.Add(v);
                    idx = vertices.Count - 1;
                    indicies[v] = idx;
                }

                if (idx == prevEdgeidx)
                {
                    continue;
                }
                
                if (i == 0)
                {
                    firstEdgeIdx = idx;
                }
                else
                {
                    edges.Add(prevEdgeidx);
                    edges.Add(idx);
                }

                prevEdgeidx = idx;
            }

            if (prevEdgeidx != firstEdgeIdx)
            {
                edges.Add(prevEdgeidx);
                edges.Add(firstEdgeIdx);
            }
        }

        public int GetEdgeA(int i)
        {
            return Edges[i * 2 + 0];
        }

        public int GetEdgeB(int i)
        {
            return Edges[i * 2 + 1];
        }

        public void Flip()
        {
            var l = Vertices.Count;
            for (var i = 0; i < l; ++i)
            {
                Vertices[i] *= -1f;
            }
        }

        public void Upload(Mesh m)
        {
            m.SetVertices(Vertices);
            m.SetTriangles(Triangles, 0);
            m.RecalculateBounds();
            m.RecalculateNormals();
        }
    }
}