using UnityEngine;
using System.Collections;

public abstract class MoveObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCol2D;
    private Rigidbody2D rigBod2D;
    private float inverseMoveTime;

	// Use this for initialization
	protected virtual void Start () {
        boxCol2D = GetComponent<BoxCollider2D>();
        rigBod2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
	}

    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        boxCol2D.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCol2D.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);
        if (hit.transform == null)
        {
            return;
        }
        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove &&  hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }
	
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float RootRemainingDistance = (transform.position - end).sqrMagnitude;
        while (RootRemainingDistance > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(rigBod2D.position, end, inverseMoveTime * Time.deltaTime);
            rigBod2D.MovePosition(newPos);
            RootRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }
    
    protected abstract void OnCantMove <T> (T component)
            where T: Component;    
}
