using System;
using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float defDistanceRay;
    public Transform laserFirePoint;
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider;
    

    private void Start()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    public IEnumerator LaserFire()
    {
        yield return new WaitForSeconds(1);
    }
    
    private void Update()
    {
        Vector2[] colliderpoints;
        colliderpoints = edgeCollider.points;
        colliderpoints[1] = laserFirePoint.position-transform.position;
        edgeCollider.points =  colliderpoints;
        
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, laserFirePoint.position);
        // idk why this shit dont work
        //ShootLaser(); 
    }

    public void stopLaser()
    {
        lineRenderer.enabled = false;
        Destroy(gameObject,0.5f);
    }

    void ShootLaser()
    {
        if (Physics2D.Raycast(transform.position, laserFirePoint.position))
        {
            RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, laserFirePoint.position);
            Draw2DRay(laserFirePoint.position, _hit.point);
        }
        else
        {
            Draw2DRay(laserFirePoint.position, laserFirePoint.position-transform.position);
        }

    }

    private void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);
    }

}

