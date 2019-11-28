using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace textura
{
    public struct Face
    {
        public Vector3 V1;
        public Vector3 V2;
        public Vector3 V3;

        public Face(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            V1 = v1;
            V2 = v2;
            V3 = v3;
        }
    }

    public struct TexturedVertex
    {
        public const int Size = (4 + 2) * 4; // size of struct in bytes

        public readonly Vector4 _position;
        public readonly Vector2 _textureCoordinate;

        public TexturedVertex(Vector4 position, Vector2 textureCoordinate)
        {
            _position = position;
            _textureCoordinate = textureCoordinate;
        }
    }

    public class IcoSphereFactory
    {
        private List<Vector3> _points;
        private int _index;
        private Dictionary<long, int> _middlePointIndexCache;

        protected TexturedVertex[] pontosEsfera; 
        public TexturedVertex[] Create(int recursionLevel)
        {
            _middlePointIndexCache = new Dictionary<long, int>();
            _points = new List<Vector3>();
            _index = 0;
            var t = (float)((1.0 + System.Math.Sqrt(5.0)) / 2.0);
            var s = 1;

            AddVertex(new Vector3(-s, t, 0));
            AddVertex(new Vector3(s, t, 0));
            AddVertex(new Vector3(-s, -t, 0));
            AddVertex(new Vector3(s, -t, 0));

            AddVertex(new Vector3(0, -s, t));
            AddVertex(new Vector3(0, s, t));
            AddVertex(new Vector3(0, -s, -t));
            AddVertex(new Vector3(0, s, -t));

            AddVertex(new Vector3(t, 0, -s));
            AddVertex(new Vector3(t, 0, s));
            AddVertex(new Vector3(-t, 0, -s));
            AddVertex(new Vector3(-t, 0, s));

            var faces = new List<Face>();

            // 5 faces around point 0
            faces.Add(new Face(_points[0], _points[11], _points[5]));
            faces.Add(new Face(_points[0], _points[5], _points[1]));
            faces.Add(new Face(_points[0], _points[1], _points[7]));
            faces.Add(new Face(_points[0], _points[7], _points[10]));
            faces.Add(new Face(_points[0], _points[10], _points[11]));

            // 5 adjacent faces 
            faces.Add(new Face(_points[1], _points[5], _points[9]));
            faces.Add(new Face(_points[5], _points[11], _points[4]));
            faces.Add(new Face(_points[11], _points[10], _points[2]));
            faces.Add(new Face(_points[10], _points[7], _points[6]));
            faces.Add(new Face(_points[7], _points[1], _points[8]));

            // 5 faces around point 3 
            faces.Add(new Face(_points[3], _points[9], _points[4]));
            faces.Add(new Face(_points[3], _points[4], _points[2]));
            faces.Add(new Face(_points[3], _points[2], _points[6]));
            faces.Add(new Face(_points[3], _points[6], _points[8]));
            faces.Add(new Face(_points[3], _points[8], _points[9]));

            // 5 adjacent faces 
            faces.Add(new Face(_points[4], _points[9], _points[5]));
            faces.Add(new Face(_points[2], _points[4], _points[11]));
            faces.Add(new Face(_points[6], _points[2], _points[10]));
            faces.Add(new Face(_points[8], _points[6], _points[7]));
            faces.Add(new Face(_points[9], _points[8], _points[1]));



            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<Face>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePoint(tri.V1, tri.V2);
                    int b = GetMiddlePoint(tri.V2, tri.V3);
                    int c = GetMiddlePoint(tri.V3, tri.V1);

                    faces2.Add(new Face(tri.V1, _points[a], _points[c]));
                    faces2.Add(new Face(tri.V2, _points[b], _points[a]));
                    faces2.Add(new Face(tri.V3, _points[c], _points[b]));
                    faces2.Add(new Face(_points[a], _points[b], _points[c]));
                }
                faces = faces2;
            }


            // // done, now add triangles to mesh
            var vertices = new List<TexturedVertex>();

            foreach (var tri in faces)
            {
                var uv1 = GetSphereCoord(tri.V1);
                var uv2 = GetSphereCoord(tri.V2);
                var uv3 = GetSphereCoord(tri.V3);
                vertices.Add(new TexturedVertex(new Vector4(tri.V1, 1), uv1));
                vertices.Add(new TexturedVertex(new Vector4(tri.V2, 1), uv2));
                vertices.Add(new TexturedVertex(new Vector4(tri.V3, 1), uv3));
            }

            this.pontosEsfera = vertices.ToArray(); 
            
            return vertices.ToArray();
        }

        private int AddVertex(Vector3 p)
        {
            _points.Add(p.Normalized());
            return _index++;
        }

        // return index of point in the middle of p1 and p2
        private int GetMiddlePoint(Vector3 point1, Vector3 point2)
        {
            long i1 = _points.IndexOf(point1);
            long i2 = _points.IndexOf(point2);
            // first check if we have it already
            var firstIsSmaller = i1 < i2;
            long smallerIndex = firstIsSmaller ? i1 : i2;
            long greaterIndex = firstIsSmaller ? i2 : i1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (_middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it

            var middle = new Vector3(
                (point1.X + point2.X) / 2.0f,
                (point1.Y + point2.Y) / 2.0f,
                (point1.Z + point2.Z) / 2.0f);

            // add vertex makes sure point is on unit sphere
            int i = AddVertex(middle);

            // store it, return index
            _middlePointIndexCache.Add(key, i);
            return i;
        }

        public static Vector2 GetSphereCoord(Vector3 i)
        {
            var len = i.Length;
            Vector2 uv;
            uv.Y = (float)(System.Math.Acos(i.Y / len) / System.Math.PI);
            uv.X = -(float)((System.Math.Atan2(i.Z, i.X) / System.Math.PI + 1.0f) * 0.5f);
            return uv;
        }

        public void desenha(){ 
            GL.LineWidth(5);
            GL.PointSize(5);
            GL.Color3(System.Drawing.Color.White);
            GL.Begin(PrimitiveType.Triangles);
            foreach (TexturedVertex ponto in pontosEsfera)
            { 
              Vector4 ve = ponto._position;
              Vector2 textura = ponto._textureCoordinate;
                // GL.Vertex3(ve.X, ve.Y, ve.Z);
                GL.TexCoord2(textura.X, textura.Y); GL.Vertex3(ve.X, ve.Y, ve.Z);

                }
             GL.End();

        }

    }
}