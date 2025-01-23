using UnityEngine;

public class TriggerParticles : MonoBehaviour
{
    // Riferimento ai Particle System
    private ParticleSystem ps1;
    private ParticleSystem ps2;
    private ParticleSystem ps3;

    void Start()
    {
        // Ottieni i Particle System dai figli del GameObject
        ps1 = gameObject.transform.GetChild(0).GetComponent<ParticleSystem>();
        ps2 = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
        ps3 = gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();

        // Controlla se i Particle System sono stati trovati
        if (ps1 == null || ps2 == null || ps3 == null)
        {
            Debug.LogError("Uno o più Particle System non trovati sul GameObject!");
        }
    }

    // Funzione per attivare i Particle System
    public void PlayParticles()
    {
        if (ps1 != null) ps1.Play();
        if (ps2 != null) ps2.Play();
        if (ps3 != null) ps3.Play();
    }

    // Funzione per fermare i Particle System
    public void StopParticles()
    {
        if (ps1 != null) ps1.Stop();
        if (ps2 != null) ps2.Stop();
        if (ps3 != null) ps3.Stop();
    }
}
