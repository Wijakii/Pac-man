using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform targetTeleporter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PacStudentController pacStudent = other.GetComponent<PacStudentController>();
            if (pacStudent != null)
            {
                pacStudent.tweener.ResetTween();
                pacStudent.transform.position = targetTeleporter.position;
                pacStudent.gridPosition = pacStudent.grid.WorldToCell(targetTeleporter.position);
                
                Collider2D targetCollider = targetTeleporter.GetComponent<Collider2D>();
                StartCoroutine(DisableCollider(targetCollider));
                
                Vector3Int nextTarget = pacStudent.TargetCell(pacStudent.currentInput);
                if (pacStudent.walkable(nextTarget))
                {
                    pacStudent.lerpTo(nextTarget);
                }
            }
            
        }
    }

    private IEnumerator DisableCollider(Collider2D collider)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(0.5f);
        collider.enabled = true;
    }
}
