using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{
    // Keeps track of all game objects and determining if they're in use
    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }

    }
    PoolObject[] poolObjects;

     // Describes the vertical spawn range for Coral objects
    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;   // Minimum height for spawn
        public float max;   // Maximum height for spawn
    }

    public GameObject Prefab;   //  Object refs
    public int poolSize;
    public float shiftSpeed;    // Speed of object movement 
    public float spawnRate;     

    // Variables for spawn information
    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; // Flag background objects in place immediately at start of game
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;// Used to scale spawn positions for varying aspect ratios  
    float spawnTimer;
    float targetAspect;

    GameManager game;

    void Awake() 
    {
        Configure();
    }

    void Start() 
    {
        // Update reference for game
        game = GameManager.Instance;
    }

    void OnEnable() 
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable() 
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed() 
    {
        // Dispose of all objects on gameOver
        for(int i=0; i<poolObjects.Length; i++) 
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }

        Configure();
    }

    void Update() 
    {
        // Don't update on gameOver
        if(game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate) 
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure() 
    {
        // Set the correct aspect ratio
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;

        poolObjects = new PoolObject[poolSize];
        for(int i=0; i<poolObjects.Length; i++) 
        {
            GameObject obj = Instantiate(Prefab) as GameObject;
            Transform t = obj.transform;
            t.SetParent(transform);
            // Initialize object off-screen
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if(spawnImmediate)
        {
            SpawnImmediate();
        }
    }
    // Spawn objects randomly
    void Spawn() 
    {
        Transform t = GetPoolObject();
        // True if poolSize is too small
        if(t == null) return;
        // Default spawn position depends on aspect ratio
        Vector3 pos = Vector3.zero;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        t.position = pos;
    }
    // Pre-spawn objects with specific initial position for a correct background at game start
    void SpawnImmediate() 
    {
        Transform t = GetPoolObject();
        // True if poolSize is too small
        if(t == null) return;
        // Default spawn position depends on aspect ratio
        Vector3 pos = Vector3.zero;
        pos.y = Random.Range(ySpawnRange.min, ySpawnRange.max);
        pos.x = (immediateSpawnPos.x * Camera.main.aspect) / targetAspect;
        t.position = pos;
        Spawn();
    }
    // Move objects across the player's screen
    void Shift() 
    {
        for(int i=0; i<poolObjects.Length; i++) 
        {
            // Translate the objects across screen by shiftSpeed
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            // Check if object position is correctly on screen
            CheckDisposeObject(poolObjects[i]);
        }
    }
    // Check if an object is off-screen
    void CheckDisposeObject(PoolObject poolObject) 
    {
        // If an object is off-screen, dispose of it
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect) 
        {
            poolObject.Dispose();
            // Set the objects position off-screen
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject() 
    {
        for(int i=0; i<poolObjects.Length; i++) 
        {
            if(!poolObjects[i].inUse) 
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }
}
