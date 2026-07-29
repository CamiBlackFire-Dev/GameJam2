using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ShowBounds : MonoBehaviour
{
    [Tooltip("Color de las líneas del límite")]
    public Color boundsColor = Color.yellow;
    
    [Tooltip("Activar simitransparencia?")]
    public bool drawSolid = false;

    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        
        if (col != null)
        {
            Gizmos.color = boundsColor;
            
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);

            if (drawSolid)
            {
                Gizmos.color = new Color(boundsColor.r, boundsColor.g, boundsColor.b, 0.1f);
                Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
