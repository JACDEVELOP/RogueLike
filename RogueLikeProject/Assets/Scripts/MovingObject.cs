using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    //Sera el tiempo que tomara nuestro objeto moverse en segundos
    public float moveTime = 0.1f;
    //Sera la capa sobre la cual vamos a comprobar la colision a medida que se avanza para ver si el espacio esta abierto para ser trasladado
    public LayerMask blockLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    // Start is called before the first frame update
    //Hacemos esto porque las funciones virtuales protegidas pueden ser anulados por sus clases heredadas 
    //Esto es util si potencialmente queremos una de las clases heredadas tener una implementacion diferente
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }
    //La palabra out hace que los argumentos sean analizado por referencia
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockLayer);
        boxCollider.enabled = true;
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    //Este siguiente metodo lo usaremos para movernos de unidades de espacio a otra
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            //mueve un espacio en linea recta
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;
}
