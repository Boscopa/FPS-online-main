using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SingleShotGun : Gun
{
    [SerializeField] Camera cam;
    [SerializeField] GameObject sparkPrefab;
    [SerializeField] Transform sparkPosition;
    [SerializeField] AudioClip gunShotSound;
    [SerializeField] int maxAmmo = 30; // Maximum ammunition
    int currentAmmo; // Current ammunition
    [SerializeField] float reloadTime = 8f; // Time it takes to reload
    bool reloading = false; // Flag to check if reloading
    PhotonView PV;
    AudioSource audioSource;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (PV.IsMine)
        {
            // Manual reload when pressing the "R" key
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !reloading)
            {
                StartCoroutine(Reload());
            }

            // Automatic reload when out of ammo
            if (currentAmmo <= 0 && !reloading)
            {
                StartCoroutine(Reload());
            }
        }
    }

    public override void Use()
    {
        if (PV.IsMine && !reloading && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);

            // Play gun shot sound
            audioSource.PlayOneShot(gunShotSound);

            // Decrement ammo
            currentAmmo--;

            // Check if we need to start reloading
            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        reloading = true;

        // Add reload animation or sound if needed

        yield return new WaitForSeconds(reloadTime);

        // Reset ammo
        currentAmmo = maxAmmo;
        reloading = false;
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            // Instantiate spark effect at the specified position
            GameObject sparkObj = Instantiate(sparkPrefab, sparkPosition.position, sparkPosition.rotation);
            Destroy(sparkObj, 2f);

            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 6f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }

}