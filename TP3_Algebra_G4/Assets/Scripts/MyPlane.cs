using UnityEngine;

//2
public class MyPlane
{
    public Vector3 normal;

    /// <summary>
    /// Guardo el triangulo que me ayuda a determinar donde el plano limita el espacio (en qué ubicacion, 
    /// no quiere decir que estos vertices sean sus limites, ya que los planos son infinitos)
    /// </summary>
    public Vector3 verA;
    public Vector3 verB;
    public Vector3 verC;

    //distance from 0,0,0
    public float distance;

    /// <summary>
    /// Plane Constructor
    /// Create the normal/distance of the Plane from 3 vertices
    /// </summary>
    /// <param name="vertex1"></param>
    /// <param name="vertex2"></param>
    /// <param name="vertex3"></param>
    public MyPlane(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        UpdatePlane(vertex1, vertex2, vertex3);
    }

    public void UpdatePlane(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        this.normal = PlaneHandler.MyCrossProduct(vertex2 - vertex1, vertex3 - vertex1).normalized;
        
        this.distance = PlaneHandler.MyDotProduct(this.normal, vertex1);

        verA = vertex1;
        verB = vertex2;
        verC = vertex3;
    }

    ///// <summary>
    ///// Plane constructor
    ///// </summary>
    ///// <param name="normal"> la normal donde apunta el plano (dentro o afuera) </param>
    ///// <param name="point"> el centro de la cara donde empieza la normal </param>
    //public MyPlane(Vector3 normal, Vector3 point)
    //{
    //    UpdatePlane(normal, point);
    //}
    //public void UpdatePlane(Vector3 normal, Vector3 point)
    //{
    //    this.normal = normal.normalized;
    //    this.distance = Vector3.Dot(this.normal, point);
    //}
    ///// <summary>
    ///// Esta del lado positivo o negativo del plano? (segun la normal)
    ///// </summary>
    ///// <param name="point"></param>
    ///// <returns></returns>
    //public bool IsOnPositiveSide(Vector3 point)
    //{
    //    //en cierta forma se transforma a la normal en coordenadas globales donde se hace el vector.
    //    //Al ser una direccion le tengo que sumar la distancia que tiene desde el punto 0,0,0 al plano para obtener la coordenada real.

    //    // El Dot es el producto escalar, es el coseno de un angulo.
    //    // Si esta inclinado para un lado (el coseno da distinto de 0, por lo que es menor o mayor a 90 grados) y da positivo, esta del lado positivo del plano.
    //    return (Vector3.Dot(this.normal, point) + distance > 0);
    //}
}   


