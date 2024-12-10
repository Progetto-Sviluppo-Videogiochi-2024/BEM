using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Cerca l'istanza nella scena
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    // Crea un nuovo GameObject e aggiungi il componente se non esiste
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }

                // Imposta questo oggetto per non essere distrutto quando si cambia scena
                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    // Awake viene chiamato prima di Start
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // Mantiene l'istanza attraverso le scene
        }
        else if (_instance != this)
        {
            // Se c'è già un'istanza, distruggi questa per evitare più istanze
            Destroy(gameObject);
        }
    }
}
