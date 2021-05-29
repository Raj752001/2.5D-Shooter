using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    public CrossHairs crosshairs;

    Camera viewCamera;
    GunController gunController;
    PlayerController controller;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        viewCamera = Camera.main;
        gunController = GetComponent<GunController> ();
        controller = GetComponent<PlayerController> ();
    }

    // Update is called once per frame
    void Update(){
        //Move Input
        Vector3 moveInput=new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //Look Input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)){
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            //print((new Vector2(point.x, point.y) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude);
            if ((new Vector2(point.x, point.y) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 6) {
                gunController.Aim(point);
            }
            
        }

        //Weapon Input
        if (Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
    }
}
