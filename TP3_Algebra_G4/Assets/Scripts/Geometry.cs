using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public static class Geometry
{
    static double maxMarginOfError = 0.001;

    public class Vector
    {
        public Vector3 start;
        public Vector3 end;

        public float x, y, z; // projection values

        public Vector(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;

            this.x = end.x - start.x;
            this.y = end.y - start.y;
            this.z = end.z - start.z;

        }

        public static Vector GetMul(Vector u, Vector v)
        {
            float x = u.y * v.z - u.z * v.y;
            float y = u.z * v.x - u.x * v.z;
            float z = u.x * v.y - u.y * v.x;

            Vector3 projection = new(x, y, z);

            Vector3 start = v.start;
            Vector3 end = start + projection;

            return new Vector(start, end);
        }
    }

    public class Plane
    {
        // Plane Equation: A * x + B * y + C * z + D = 0
        public float a;
        public float b;
        public float c;
        public float d;

        public Plane(float a, float b, float c, float d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

        }

        public Plane(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector v = new(vertex1, vertex2);

            Vector u = new(vertex1, vertex3);

            Vector n = Geometry.Vector.GetMul(v, u);

            this.a = n.x;
            this.b = n.y;
            this.c = n.z;

            //D = -(A * x0 + B * y0 + c * z0).
            this.d = -(this.a * vertex1.x + this.b * vertex1.y + this.c * vertex1.z);

        }
        public static Plane GetNegative(Plane p1)
        {
            Plane p = new(
                -p1.a,
                -p1.b,
                -p1.c,
                -p1.d
                );

            return p;
        }

        public static float GetMul(Plane plane, Vector3 scalar)
        {
            return scalar.x * plane.a + scalar.y * plane.b + scalar.z * plane.c + plane.d;
        }
    }

    public class Face
    {
        // Vertices in one face
        List<Vector3> vertices;

        // Indexes
        List<int> indexes;

        //number of vertices
        int n;

        public Face(List<Vector3> vertices, List<int> indexes)
        {
            n = vertices.Count;

            for (int i = 0; i < n; i++)
            {
                this.vertices.Add(vertices[i]);
                this.indexes.Add(indexes[i]);
            }

        }
    }

    public class Polygon
    {
        public List<Vector3> vertices;
        public List<int> indexes;
        public int n;

        public Polygon(List<Vector3> vertices)
        {
            this.vertices = new List<Vector3>();
            this.indexes = new List<int>();     

            n = vertices.Count;

            for (int i = 0; i < n; i++)
            {
                this.vertices.Add(vertices[i]);
                this.indexes.Add(i);
            }

            Debug.Log("create poly");
        }
    }

    public class PolygonProcess
    {
        Polygon polygon;

        //Bounds
        float xMin, xMax,
              yMin, yMax,
              zMin, zMax;

        //Faces
        List<Face> faces;

        List<Plane> planes;

        int facesAmount;

        // Maximum point to face plane distance error, point is considered in the face plane if its distance is less than this error
        float marginOfError;

        void SetBoundaries()
        {
            List<Vector3> vertices = polygon.vertices;

            int n = polygon.n;

            this.xMin = vertices[0].x;
            this.xMax = vertices[0].x;
            this.yMin = vertices[0].y;
            this.yMax = vertices[0].y;
            this.zMin = vertices[0].z;
            this.zMax = vertices[0].z;

            for (int i = 1; i < n; i++)
            {
                if (vertices[i].x < xMin) this.xMin = vertices[i].x;
                if (vertices[i].y < yMin) this.yMin = vertices[i].y;
                if (vertices[i].z < zMin) this.zMin = vertices[i].z;
                if (vertices[i].x > xMax) this.xMax = vertices[i].x;
                if (vertices[i].y > yMax) this.yMax = vertices[i].y;
                if (vertices[i].z > zMax) this.zMax = vertices[i].z;
            }
        }

        void SetMarginOfError()
        {
            marginOfError = (float)((Mathf.Abs(xMin) + Mathf.Abs(xMax) +
                              Mathf.Abs(yMin) + Mathf.Abs(yMax) +
                              Mathf.Abs(zMin) + Mathf.Abs(zMax)) / 6 * maxMarginOfError);
        }

        bool ListEquals(List<int> list1, List<int> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                    return false;
            }

            return true;
        }

        bool ContainsVector(List<List<int>> listOfItems, List<int> items)
        {
            items.Sort();
            for (int i = 0; i < listOfItems.Count; i++)
            {
                List<int> temp = listOfItems[i];
                if (temp.Count() == items.Count())
                {
                    items.Sort();
                    if (ListEquals(temp, items))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void SetFaces()
        {
            List<Vector3> vertices = polygon.vertices;

            int n = polygon.n;

            //face planes
            List<Plane> facePlanes = new();

            // 2d vertices indexes, first dimension is face index,
            // second dimension is vertice indexes in one face
            List<List<int>> faceVerticesIndexes = new();

            for (int i = 0; i < n; i++)
            {
                // triangle point 1
                Vector3 triangle_1 = vertices[i];

                for (int j = i + 1; j < n; j++)
                {
                    // triangle point 2
                    Vector3 triangle_2 = vertices[j];

                    for (int k = j + 1; k < n; k++)
                    {
                        // triangle point 3
                        Vector3 triangle_3 = vertices[k];

                        Plane trianglePlane = new(triangle_1, triangle_2, triangle_3);

                        int onLeftCount = 0;
                        int onRightCount = 0;

                        List<int> indexPointInPlane = new();

                        for (int l = 0; l < n; l++)
                        {
                            // check any vertices other than the 3 triangle vertices
                            if (l != i && l != j && l != k)
                            {
                                Vector3 vertex = vertices[l];

                                float dis = Plane.GetMul(trianglePlane, vertex);

                                // add next vertice that is in the triangle plane
                                if (Mathf.Abs(dis) < this.marginOfError)
                                {
                                    indexPointInPlane.Add(l);
                                }
                                else
                                {
                                    if (dis < 0)
                                    {
                                        onLeftCount++;
                                    }
                                    else
                                    {
                                        onRightCount++;
                                    }
                                }
                            }
                        }

                        if (onLeftCount == 0 || onRightCount == 0)
                        {
                            List<int> faceVerticeIndexInOneFace = new();

                            // add 3 triangle vertices to the triangle plane
                            faceVerticeIndexInOneFace.Add(i);
                            faceVerticeIndexInOneFace.Add(j);
                            faceVerticeIndexInOneFace.Add(k);

                            // add other same plane vetirces in this triangle plane                
                            for (int p = 0; p < indexPointInPlane.Count; p++)
                            {
                                faceVerticeIndexInOneFace.Add(indexPointInPlane[p]);
                            }

                            // check if it is a new face
                            if (!ContainsVector(faceVerticesIndexes, faceVerticeIndexInOneFace))
                            {
                                // add the new face
                                faceVerticesIndexes.Add(faceVerticeIndexInOneFace);

                                // add the new face plane
                                if (onRightCount == 0)
                                {
                                    facePlanes.Add(trianglePlane);
                                }
                                else if (onLeftCount == 0)
                                {
                                    facePlanes.Add(Plane.GetNegative(trianglePlane));
                                }
                            }

                            faceVerticeIndexInOneFace.Clear();

                        }

                        indexPointInPlane.Clear();

                    } // k loop
                } // j loop        
            } // i loop

            // set number of faces           
            facesAmount = faceVerticesIndexes.Count();

            for (int i = 0; i < facesAmount; i++)
            {
                // set face planes
                planes.Add(new(facePlanes[i].a, facePlanes[i].b,
                               facePlanes[i].c, facePlanes[i].d));

                // face vertices
                List<Vector3> faceVertices = new();

                // face vertices indexes
                List<int> fvIndexes = new();

                // number of vertices of the face
                int count = faceVerticesIndexes[i].Count();

                for (int j = 0; j < count; j++)
                {
                    fvIndexes.Add(faceVerticesIndexes[i][j]);

                    faceVertices.Add(new(vertices[fvIndexes[j]].x,
                                         vertices[fvIndexes[j]].y,
                                         vertices[fvIndexes[j]].z));
                }

                // set faces
                faces.Add(new(faceVertices, fvIndexes));
            }

            facePlanes.Clear();
            faceVerticesIndexes.Clear();
        }

        public bool IsPointInPolygon(Vector3 point)
        {

            for (int i = 0; i < facesAmount; i++)
            {
                double dis = Plane.GetMul(planes[i], point);

                // If the point is in the same half space with normal vector 
                // for any face of the 3D convex polygon, then it is outside of the 3D polygon        
                if (dis > 0)
                {
                    return false;
                }
            }

            // If the point is in the opposite half space with normal vector for all faces,
            // then it is inside of the 3D polygon
            return true;
        }

        public PolygonProcess(Polygon polygon)
        {
            this.polygon = polygon;

            this.faces = new List<Face>();
            this.planes = new List<Plane>();

            SetBoundaries();
            SetMarginOfError();
            SetFaces();
        }
    }
}
