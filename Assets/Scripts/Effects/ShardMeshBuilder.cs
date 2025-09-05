using UnityEngine;

/// <summary>
/// Builds a grid mesh covering NDC-like space [-1,1] x [-1,1], with UV 0..1 and per-cell id.
/// IMPORTANT: Duplicates vertices per cell so each shard moves rigidly without sharing verts.
/// Stores:
///  - uv  : standard 0..1 UV
///  - uv2 : x = shard id (float)
///  - uv3 : (uCenter, vCenter) of the shard (same for all 4 verts in the cell)
/// </summary>
public static class ShardMeshBuilder
{
    public static Mesh BuildGrid(int cols, int rows)
    {
        int quadCount = cols * rows;
        int vertCount = quadCount * 4; // 4 unique verts per cell

        var verts = new Vector3[vertCount];
        var uvs   = new Vector2[vertCount];
        var uv2   = new Vector2[vertCount];
        var uv3   = new Vector2[vertCount];
        var tris  = new int[quadCount * 6];

        int vi = 0;
        int ti = 0;
        int shardId = 0;

        for (int y = 0; y < rows; y++)
        {
            float v0 = (float)y / rows;
            float v1 = (float)(y + 1) / rows;
            float vc = (v0 + v1) * 0.5f;

            for (int x = 0; x < cols; x++)
            {
                float u0 = (float)x / cols;
                float u1 = (float)(x + 1) / cols;
                float uc = (u0 + u1) * 0.5f;

                // Positions in NDC-like [-1,1]
                Vector3 p00 = new Vector3(u0 * 2f - 1f, v0 * 2f - 1f, 0f);
                Vector3 p10 = new Vector3(u1 * 2f - 1f, v0 * 2f - 1f, 0f);
                Vector3 p01 = new Vector3(u0 * 2f - 1f, v1 * 2f - 1f, 0f);
                Vector3 p11 = new Vector3(u1 * 2f - 1f, v1 * 2f - 1f, 0f);

                // Four unique vertices per quad
                verts[vi + 0] = p00; uvs[vi + 0] = new Vector2(u0, v0);
                verts[vi + 1] = p10; uvs[vi + 1] = new Vector2(u1, v0);
                verts[vi + 2] = p01; uvs[vi + 2] = new Vector2(u0, v1);
                verts[vi + 3] = p11; uvs[vi + 3] = new Vector2(u1, v1);

                float idf = shardId + 0.5f;
                uv2[vi + 0].x = idf;
                uv2[vi + 1].x = idf;
                uv2[vi + 2].x = idf;
                uv2[vi + 3].x = idf;

                Vector2 centerUV = new Vector2(uc, vc);
                uv3[vi + 0] = centerUV;
                uv3[vi + 1] = centerUV;
                uv3[vi + 2] = centerUV;
                uv3[vi + 3] = centerUV;

                // Two triangles per quad: (0,2,1) and (1,2,3)
                tris[ti + 0] = vi + 0;
                tris[ti + 1] = vi + 2;
                tris[ti + 2] = vi + 1;
                tris[ti + 3] = vi + 1;
                tris[ti + 4] = vi + 2;
                tris[ti + 5] = vi + 3;

                vi += 4;
                ti += 6;
                shardId++;
            }
        }

        var m = new Mesh();
        if (vertCount > 65000)
            m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        m.vertices = verts;
        m.uv = uvs;
        m.uv2 = uv2;
        // If uv3 property is unavailable on your Unity version, use SetUVs(2/3) variants.
        m.uv3 = uv3;
        m.triangles = tris;

        // Extremely large bounds to avoid CPU culling when shader writes clip-space positions
        m.bounds = new Bounds(Vector3.zero, Vector3.one * 100000f);
        return m;
    }
}
