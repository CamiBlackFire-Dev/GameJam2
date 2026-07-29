using UnityEngine;

/// <summary>
/// Script de ayuda para el editor. Dibuja siempre los límites de un Collider2D en la vista de escena,
/// incluso cuando el objeto no está seleccionado.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ShowBounds : MonoBehaviour
{
    [Tooltip("Color del borde del área.")]
    public Color boundsColor = Color.yellow;
    
    [Tooltip("Dibuja también el interior del área de forma semi-transparente.")]
    public bool drawSolid = false;

    // Esta función nativa de Unity dibuja formas en la pestaña Scene para facilitar el diseño de niveles
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();
        
        if (col != null)
        {
            Gizmos.color = boundsColor;
            
            // Dibujar el contorno exterior usando el tamaño real del collider en el mundo
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);

            if (drawSolid)
            {
                // Dibujar el interior con el mismo color pero mucha transparencia (Alpha 0.1f)
                Gizmos.color = new Color(boundsColor.r, boundsColor.g, boundsColor.b, 0.1f);
                Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
