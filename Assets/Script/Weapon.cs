using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id, prefabId;
    public float damage;
    public int count;
    public float speed;
    float timer;
    Player player;
    private void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive) return;


        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;
                if(timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

    }
    public void Init(ItemData data)
    {
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        //property set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for (int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed;
                Batch();
                break;

            default:
                speed = 1f * Character.WeaponRate;
                break;
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);

    }

    private void Fire()
    {
        if (!player.scanner.nearestTarget)
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);

    }

    public void LevelUp(float damage, int count)
    {
        this.count += count;
        this.damage = damage;
        if(id == 0)
        {
            Batch();
        }
        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);

    }
    void Batch()
    {
        for(int i = 0; i < count; i++)
        {
            Transform bullet;
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {               
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotationVec = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotationVec);
            bullet.Translate(bullet.up * 2f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);//-100 per vo han (dan xuyen thau tat ca enemy)
        }
    }
}
