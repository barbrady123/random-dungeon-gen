using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool _isMoving;

    public float MoveSpeed;

    void Start()
    {
        _isMoving = false;
    }

    void Update()
    {
        if (_isMoving)
            return;

		int rawX = Input.GetAxisRaw(Global.Inputs.AxisHorizontal).RawDirection();
		int rawY = Input.GetAxisRaw(Global.Inputs.AxisVertical).RawDirection();

        if ((rawX == 0) && (rawY == 0))
            return;

        if (rawX != 0f)
        {
            transform.localScale =
                new Vector3(
                    rawX,
                    transform.localScale.y,
                    transform.localScale.z);
        }

        Vector3 targetDest = Vector3.zero;

        if (rawX != 0)
        {
            targetDest = new Vector3(transform.position.x + rawX, transform.position.y, transform.position.z);
        }
        else
        {
            targetDest = new Vector3(transform.position.x, transform.position.y + rawY, transform.position.z);
        }

        // Seems like there could be a race condition here, probably should test at least one additional time during the movement process?
        if (!TestCollision(targetDest))
        {
            StartCoroutine(SmoothMove(targetDest));
        }
    }

    private IEnumerator SmoothMove(Vector3 target)
    {
        _isMoving = true;

        while (Vector3.Distance(transform.position, target) > 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * this.MoveSpeed);
            yield return null;
        }

        _isMoving = false;
    }

    private bool TestCollision(Vector3 target)
    {
        var hitCollider = Physics2D.OverlapBox(
            target,
            Vector3.one * 0.8f,
            0,
            LayerMask.GetMask("Wall", "Enemy"));

        return hitCollider != null;
    }
}
