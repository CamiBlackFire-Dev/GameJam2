using UnityEngine;

// Este script es solo para ayudarnos en Unity. Nos dibuja una cajita amarilla 
// para ver dónde están los colliders sin tener que seleccionarlos.
[RequireComponent(typeof(Collider2D))]
public class ShowBounds : MonoBehaviour
{
    [Tooltip("El color de la cajita que se dibuja")]
    public Color boundsColor = Color.yellow;

    [Tooltip("Si queremos que la cajita tenga relleno o solo las líneas")]
    public bool drawSolid = false;

    // Unity usa esto para dibujar cosas extra en la ventana de Scene
    private void OnDrawGizmos()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            Gizmos.color = boundsColor;

            // Dibujamos el borde exacto del collider
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);

            if (drawSolid)
            {
                // Le ponemos un poco de transparencia para pintar el interior
                Gizmos.color = new Color(boundsColor.r, boundsColor.g, boundsColor.b, 0.1f);
                Gizmos.DrawCube(col.bounds.center, col.bounds.size);
            }
        }
    }
}
