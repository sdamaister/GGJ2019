using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawnController : MonoBehaviour
{
    public float MinDelay = 10.0f;
    public float MaxDelay = 20.0f;
    public float FirstDelay = 1.0f;
    public int InitialAmount = 3;

    public String SpawnPointsTag = "spawn";
    public GameObject SpawneableObject;

    public int PoolSize = 10;

    private GameObject LastSpawn = null;
    private float NextSpawnTs;
    private int spawnPointsCount = 0;
    private List<GameObject> EmptySpawnPoints;
    private List<GameObject> ItemPool;

    // Start is called before the first frame update
    void Start()
    {
        NextSpawnTs = FirstDelay;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(SpawnPointsTag);
        EmptySpawnPoints = new List<GameObject>(spawnPoints);

        ItemPool = new List<GameObject>(PoolSize);
        for (int i = 0; i < PoolSize; ++i)
        {
            GameObject item = Instantiate(SpawneableObject);
            item.name = SpawneableObject.name + " #" + i;
            item.GetComponent<Pickable>().Spawner = this;

            item.transform.parent = transform;

            item.transform.localPosition = Vector3.zero;
            item.transform.rotation = Quaternion.identity;

            item.SetActive(false);
            ItemPool.Add(item);
        }

        spawnPointsCount = spawnPoints.Length;
    }

    // Update is called once per frame
    void Update()
    {
        NextSpawnTs -= Time.deltaTime;

        if (NextSpawnTs <= 0.0f)
        {
            if (EmptySpawnPoints.Count > 0 && ItemPool.Count > 0)
            {
                SpawnObject(SelectNextSpawnPoint());
                NextSpawnTs = Random.Range(MinDelay, MaxDelay);
            }
        }
    }

    public void ItemPickedUp(GameObject from)
    {
        EmptySpawnPoints.Add(from);

        if (EmptySpawnPoints.Count == 1 && ItemPool.Count > 0)
        {
            NextSpawnTs = Random.Range(MinDelay, MaxDelay);
        }
    }

    public void ItemDestroyed(GameObject item)
    {
        item.SetActive(false);
        item.transform.parent = transform;

        item.transform.localPosition = Vector3.zero;
        item.transform.rotation = Quaternion.identity;

        ItemPool.Add(item);

        if (ItemPool.Count == 1 && EmptySpawnPoints.Count > 0)
        {
            NextSpawnTs = Random.Range(MinDelay, MaxDelay);
        }
    }

    public void SpawnInitial()
    {
        int amountToSpawn = Mathf.Max(0, InitialAmount - (spawnPointsCount - EmptySpawnPoints.Count));
        for (int i = 0; i < amountToSpawn; ++i)
        {
            SpawnObject(SelectNextSpawnPoint());
        }
    }

    private GameObject SelectNextSpawnPoint()
    {
        GameObject selected = null;
        do
        {
            int which = Random.Range(0, EmptySpawnPoints.Count);
            selected = EmptySpawnPoints[which];
        } while (selected == LastSpawn && (EmptySpawnPoints.Count > 1) );

        EmptySpawnPoints.Remove(selected);
        LastSpawn = selected;
        return selected;
    }

    private void SpawnObject(GameObject where)
    {
        GameObject item = ItemPool[0];
        ItemPool.RemoveAt(0);

        GameObject spawned = new GameObject(item.name + " Pickup");
        spawned.tag = "Pickup";

        PickupTrigger trigger = spawned.AddComponent<PickupTrigger>();
        trigger.SpawnedFrom = where;

        trigger.SetPickupObject(item);
        trigger.transform.position = where.transform.position;
        item.transform.SetParent(trigger.transform, false);

        item.SetActive(true);
    }
}
