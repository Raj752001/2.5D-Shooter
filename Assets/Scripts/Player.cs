using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed = 5;

    public CrossHairs crosshairs;
    public Joystick moveJoystick;
    public Joystick aimJoystick;
    public Transform weaponHold;

    Camera viewCamera;
    GunController gunController;
    PlayerController controller;

    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
    }

    private void Awake()
    {
        viewCamera = Camera.main;
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber) {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    // Update is called once per frame
    void Update(){
        //Move Input
        Vector3 moveInput = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical) + new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //Vector3 moveInput = new Vector3(moveJoystick.Horizontal, 0, moveJoystick.Vertical);
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        //Look Input
        Vector3 aimInput = new Vector3(aimJoystick.Horizontal, 0, aimJoystick.Vertical);
        if (aimInput.magnitude != 0) {
            Vector3 aimPoint = weaponHold.position + aimInput.normalized * 10f;
            controller.LookAt(aimPoint);
            gunController.Aim(aimPoint);
        }
        //Debug.DrawRay(weaponHold.position, weaponHold.forward *10f, Color.red);


        //Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        //Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        //float rayDistance;

        //if (groundPlane.Raycast(ray, out rayDistance)){
        //    Vector3 point = ray.GetPoint(rayDistance);
        //    //Debug.DrawLine(ray.origin, point, Color.red);
        //    controller.LookAt(point);
        //    crosshairs.transform.position = point;
        //    crosshairs.DetectTargets(ray);
        //    //print((new Vector2(point.x, point.y) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude);
        //     if ((new Vector2(point.x, point.y) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 6) {
        //        gunController.Aim(point);
        //    }

        //}

        //Weapon Input
        //if (Input.GetMouseButton(0)) {
        // gunController.OnTriggerHold();
        //}
        //bool brustReloaded = true;
        if (aimJoystick.Direction.magnitude >= 1)
        {
            gunController.OnTriggerHold();
        //    brustReloaded = false;
        //    print(brustReloaded);
        }
       // else {
        //    if (!brustReloaded)
        //    {
        //        gunController.OnTriggerRelease();
         //       print(brustReloaded);
        //        brustReloaded = true;
        //    }
       // }
       //if (Input.GetMouseButtonUp(0))
       // {
       //     gunController.OnTriggerRelease();
       // } 
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    gunController.Reload();
        //}
    }

    public override void Die()
    {
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}
