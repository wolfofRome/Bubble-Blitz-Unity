using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Edit Gravity Scale for better float
// Original gravity scale value = 1
[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    public AudioSource tapAudio;
    public AudioSource scoreAudio;
    public AudioSource dieAudio;

    Rigidbody2D rigidbody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager game;
 

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0,0,-90);
        forwardRotation = Quaternion.Euler(0,0,35);
        game = GameManager.Instance;
        rigidbody.simulated = false;
    }

    void OnEnable() 
    {
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;

    }

    void OnDisable() 
    {
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameStarted() 
    {
        // Reset velocity to 0 when game starts
        rigidbody.velocity = Vector3.zero;
        // Reactivate physics on start
        rigidbody.simulated = true;
    }

    void OnGameOverConfirmed() 
    {
        // Set player back to start position 
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        // Don't update player rotation if GameOver is true
        if (game.GameOver) return;

        // Tap on mobile, Left/Right Click on PC
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) 
        {
            // play tap sound
            tapAudio.Play();

            // Snap up to forward rotation on every tap
            transform.rotation = forwardRotation;

            // Zero out the velocity during the fall and swim up
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);
        }
        
        // Handle downward rotation of the fish as it falls
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if(col.gameObject.tag == "ScoreZone") 
        {
            // register score event
            OnPlayerScored(); // event sent to GameManager
            // play score sound
            scoreAudio.Play();
        }

        if(col.gameObject.tag == "DeadZone") 
        {
            // freeze the fish
            rigidbody.simulated = false;
            // register dead event
            OnPlayerDied(); // event sent to GameManager
            // play death sound
            dieAudio.Play();
        }
    }
}