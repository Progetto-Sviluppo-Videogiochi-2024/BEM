using UnityEngine;

public class MoveStefano : MonoBehaviour
{
    public Animator playerAnim; // Animator del player
    public Rigidbody playerRigid; // Rigidbody del player
    public float idleTimeThreshold = 3.0f; // Tempo in secondi per considerare il giocatore inattivo
    private float elapsedTime = 0.0f; // Tempo trascorso da quando il giocatore ha smesso di muoversi
    private Vector3 lastPosition; // Ultima posizione del giocatore
    private bool isIdleTriggerActivated = false; // Trigger booleano per controllare se il giocatore è inattivo
    public float w_speed, wb_speed, olw_speed, rn_speed, ro_speed; // Velocità di movimento, velocità di corsa, velocità di rotazione
    public bool walking; // Flag per il movimento del player
    public Transform playerTrans; // Trasform del player

    void Start()
    {
        lastPosition = transform.position;
    }

    void FixedUpdate()
    {
        /* Per il movimento del player */
        if (Input.GetKey(KeyCode.W))
        {
            playerRigid.velocity = transform.forward * w_speed * Time.deltaTime;

        }
        if (Input.GetKey(KeyCode.S))
        {
            playerRigid.velocity = -transform.forward * wb_speed * Time.deltaTime;
        }
    }

    //Da implemenetare con il file di Christian
    /*
    private void ComputeVelocity()
    {
        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += Vector3.forward;
        if (Input.GetKey(KeyCode.S)) direction += Vector3.back;
        //if (Input.GetKey(KeyCode.A)) direction += Vector3.left;
        //if (Input.GetKey(KeyCode.D)) direction += Vector3.right;

        direction = direction.normalized;

        Vector3 forwardDirection = mainCamera.transform.forward;
        forwardDirection.y = 0; // Mantenere il movimento orizzontale
        forwardDirection = forwardDirection.normalized;

        Vector3 rightDirection = mainCamera.transform.right;
        rightDirection.y = 0; // Mantenere il movimento orizzontale
        rightDirection = rightDirection.normalized;

        Vector3 moveDirection = forwardDirection * direction.z + rightDirection * direction.x;

        if (playerAnim.GetBool("isAim")) playerRigid.velocity = aimMoveSpeed * Time.deltaTime * moveDirection;
        else playerRigid.velocity = Time.deltaTime * w_speed * moveDirection;
    }
    */

    void Update()
    {
        /* Per le animazioni del player */
        if (IsIdle()) // Se il giocatore è inattivo per un certo periodo di tempo (idleTimeThreshold)
        {
            playerAnim.ResetTrigger("JogForward");
            playerAnim.ResetTrigger("JogBackward");
            playerAnim.ResetTrigger("Running");
            playerAnim.SetTrigger("Inactive");
            //steps1.SetActive(false);
            //steps2.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.W)) // Se il giocatore preme il tasto W per muoversi in avanti
        {
            playerAnim.ResetTrigger("Inactive");
            playerAnim.ResetTrigger("Idle");
            playerAnim.SetTrigger("JogForward");
            walking = true;
            //steps1.SetActive(true); suono dei passi
        }
        if (Input.GetKeyUp(KeyCode.W)) // Se il giocatore rilascia il tasto W per smettere di muoversi in avanti
        {
            playerAnim.ResetTrigger("JogForward");
            playerAnim.SetTrigger("Idle");
            walking = false;
            //steps1.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.S)) // Se il giocatore preme il tasto S per muoversi all'indietro
        {
            playerAnim.ResetTrigger("Inactive");
            playerAnim.ResetTrigger("Idle");
            playerAnim.SetTrigger("JogBackward");
            //steps1.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.S)) // Se il giocatore rilascia il tasto S per smettere di muoversi all'indietro
        {
            playerAnim.ResetTrigger("JogBackward");
            playerAnim.SetTrigger("Idle");
            //steps1.SetActive(false);
        }

        /* Per la rotazione del player */
        if (Input.GetKey(KeyCode.A)) // Se il giocatore preme il tasto A per ruotare a sinistra
        {
            playerTrans.Rotate(0, -ro_speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D)) // Se il giocatore preme il tasto D per ruotare a destra
        {
            playerTrans.Rotate(0, ro_speed * Time.deltaTime, 0);
        }

        /* Per lo sprint (corsa) del player */
        if (walking == true)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift)) // Se il giocatore preme il tasto Shift sinistro per correre
            {
                //steps1.SetActive(false);
                //steps2.SetActive(true);
                w_speed = w_speed + rn_speed * 2f;
                playerAnim.ResetTrigger("JogForward");
                playerAnim.SetTrigger("Running");
            }
            if (Input.GetKeyUp(KeyCode.LeftShift)) // Se il giocatore rilascia il tasto Shift sinistro per smettere di correre
            {

                //steps1.SetActive(true);
                //steps2.SetActive(false);
                w_speed = olw_speed;
                playerAnim.ResetTrigger("Running");
                playerAnim.SetTrigger("JogForward");
            }
        }
    }

    bool IsIdle()
    {
        // Controlla se il giocatore si è mosso o se il tempo trascorso ha già superato la soglia
        if (transform.position != lastPosition)
        {
            elapsedTime = 0.0f; // Reimposta il timer se il giocatore si muove
            isIdleTriggerActivated = false; // Reimposta il trigger
        }
        else
        {
            elapsedTime += Time.deltaTime; // Incrementa il timer se il giocatore non si muove
        }

        lastPosition = transform.position; // Aggiorna l'ultima posizione

        // Controlla se il tempo trascorso supera la soglia
        if (elapsedTime >= idleTimeThreshold && !isIdleTriggerActivated)
        {
            isIdleTriggerActivated = true; // Imposta il trigger booleano a true
            return true; // Il giocatore è inattivo
        }
        return false; // Il giocatore è attivo
    }
}
