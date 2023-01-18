    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public Camera playerCam;
    public float ropeMultiplier = 1;
    public float maxLaunchDist;
    public Transform player;
    public Rigidbody2D rb;
    public SpringJoint2D sj;
    public LayerMask metal;
    public float grappleMult;
    public LineRenderer line;
    public float releaseDistance;


    private RaycastHit2D hit;
    private Rigidbody2D rbHit;
    private float grappleTime = 0;
    private Vector3 mousePos;
    private bool hasShot=false;
    private Vector2 direction;
    private bool hasExtended = false;
    private bool canGrapple = false;


    void Start()
    {
        
    }

    void Update()
    {

        //Left click initiates grapple
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            canGrapple = true;
        }

        if(Input.GetKey(KeyCode.Mouse0) && canGrapple)
        {
            if (!hasShot)
            {
                mousePos = Tools.GetMouseWorldPositionPerspective(Input.mousePosition, playerCam);
                direction = (mousePos - transform.position).normalized;
                hit = Physics2D.Raycast(transform.position, direction, maxLaunchDist);
                line.SetPosition(0, player.position);
                hasShot = true;    
            }
            
            if (hit.collider != null && hit.collider.gameObject.tag == "Metallic")
            {
                line.SetPosition(0, player.position);
                line.SetPosition(1, hit.point);
                line.enabled = true;
                rbHit = hit.rigidbody;

                Shoot();
            }
            else
            {
                FreeGrapple();
            }
        }  

        //Releasing left click retracts grappler
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ungrapple();
        }
    }

    void Shoot()
    {
        sj.enabled = true;
        sj.connectedBody = rbHit;
        sj.connectedAnchor = (hit.point);

        Launch();
    }

    void Launch()
    {
        Vector2 grappleSpeed = (hit.point - new Vector2(player.position.x,player.position.y)).normalized;
        rb.velocity = grappleSpeed*grappleMult;

        //releases when close to anchor point
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        if(Vector2.Distance(sj.connectedAnchor, position) < releaseDistance)
        {
            Debug.Log("aa");
            Ungrapple();
            canGrapple = false;
        }
    }

    private void FreeGrapple()
    {
        Vector2 target;        if (Vector2.Distance(mousePos, hit.point) < maxLaunchDist)
        {
            target = hit.point;
            //Debug.Log("hit");
        }
        else
        {
            target = new Vector2(player.position.x,player.position.y) + (direction * maxLaunchDist);
            //Debug.Log("miss");
        }

        grappleTime = Mathf.Clamp(grappleTime, 0, 1);

        if (!hasExtended){
            grappleTime += Time.deltaTime * ropeMultiplier;
            float lerpVal = Mathf.InverseLerp(0, 1, grappleTime);
            line.enabled = true;
            line.SetPosition(0, player.position);
            line.SetPosition(1, Vector2.Lerp(player.position, target, lerpVal));

            if (new Vector2(line.GetPosition(1).x, line.GetPosition(1).y) == target)
            {
                hasExtended = true;
            }
        }
        else
        {
            grappleTime += Time.deltaTime * -ropeMultiplier;
            float lerpVal = Mathf.InverseLerp(0, 1, grappleTime);
            line.SetPosition(0, player.position);
            line.SetPosition(1, Vector2.Lerp(player.position, target, lerpVal));
            if(line.GetPosition(1)== line.GetPosition(0))
            {
                
            }
        }
    }

    void Ungrapple()
    {
        line.enabled = false;
        sj.enabled = false;
        rb.velocity = Vector3.zero;
        hasShot = false;
        hasExtended = false;
        grappleTime = 0;
    }
}
