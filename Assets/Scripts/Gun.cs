using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode {Auto, Burst, Single};
    public FireMode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float sBetweenShots = .100f;
    public float muzzleVelocity = 35;
    public int burstCount = 3;
    public int projectilesPerMag = 30;
    public float reloadTime = .3f;

    [Header("Recoil")]
    public Vector2 kickMinMax = new Vector2(.05f, .2f);
    public Vector2 recoilAngleMinMax = new Vector2(3,5);
    public float recoilMoveSettleTime = .1f;
    public float recoilAngeSettleTime = .1f;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjection;
    public AudioClip shootAudio;
    public AudioClip reloadAudio;
    MuzzleFlash muzzleFlash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shootsRemainingInBurst;
    float nextBurstTime;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 smoothDampVelocity;
    float recoilAngle;
    float recoilAngleSmoothDampVelcotiy;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shootsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    private void LateUpdate()
    {
        // To animate reciol
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero,ref smoothDampVelocity, recoilMoveSettleTime);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilAngleSmoothDampVelcotiy, recoilAngeSettleTime);
        transform.eulerAngles += Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag == 0) {
            Reload();
        }
    }

    void Shoot()
    {
        if (!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0)
        {
            if (fireMode == FireMode.Burst) {
                if (shootsRemainingInBurst == 0) {
                    return;
                }
                shootsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }
            for (int i = 0; i < projectileSpawn.Length; i++) {
                if (projectilesRemainingInMag == 0) {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + sBetweenShots;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }
            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x, kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);

            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload() {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag) {
            StartCoroutine("AnimateReload");
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
        
    }

    IEnumerator AnimateReload() {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float percent = 0;
        float reloadSpeed = 1/reloadTime;
        Vector3 initialRot = transform.localEulerAngles;
        float maxRecoilAngle = 30;

        while (percent < 1) {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-(Mathf.Pow(percent, 2)) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxRecoilAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;
            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
        shootsRemainingInBurst = burstCount;
    }

    public void Aim(Vector3 aimPoint) {
        if (!isReloading) {
            transform.LookAt(aimPoint);
        }
    }

    public void OnTriggerHold() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shootsRemainingInBurst = burstCount;
    }
}
