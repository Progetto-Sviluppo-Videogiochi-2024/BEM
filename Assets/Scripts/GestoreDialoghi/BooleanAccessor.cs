using UnityEngine;

public class BooleanAccessor : MonoBehaviour
{
    // Singleton
    public static BooleanAccessor istance;

    // Bools di Jackob
    bool cartello = false;
    bool tolto = false;

    // Bools Uomo Baita
    bool capra = false;
    bool capraDone = false;

    //Bools Gaia
    bool fiore = false;
    bool soluzione = false;


    //E' necessario che tutte le variabili booleane create nel dialogue editor siano settate a false.
    public void SetBoolOnDialogueE(string nomeBool)
    {
        switch (nomeBool)
        {
            case "cartello":
                cartello = true;
                break;
            case "tolto":
                tolto = true;
                break;
            case "capra":
                capra = true;
                break;
            case "capraDone":
                capraDone = true;
                break;
            case "fiore":
                fiore = true;
                break;
            case "soluzione":
                soluzione = true;
                break;
            default:
                Debug.LogWarning("BooleanAccessor: nomeBool non riconosciuto - " + nomeBool);
                break;
        }
    }

    //Permette agli scripts dei dialoghi di poter accedere alle variabili booleane globali
    public bool GetBoolFromThis(string nomeBool)
    {
        return nomeBool switch
        {
            "cartello" => cartello,
            "tolto" => tolto,
            "capra" => capra,
            "capraDone" => capraDone,
            "fiore" => fiore,
            "soluzione" => soluzione,
            _ => false,
        };
    }

    void Awake()
    {
        if (istance == null)
        {
            istance = this;
            DontDestroyOnLoad(gameObject);
            // Debug.Log("BooleanAccessor inizializzato");
        }
        else if (istance != this)
        {
            Debug.Log("Un'altra istanza di BooleanAccessor è stata distrutta");
            Destroy(gameObject);
        }
    }

    // For Debug

    // void Update()
    // {
    //     //Debug per vedere se i valori delle variabili booleane globali
    //     if (Input.GetKeyDown(KeyCode.K))
    //     {
    //         Debug.Log("capra " + capra);
    //         Debug.Log("capra_function: " + BooleanAccessor.istance.GetBoolFromThis("capra"));
            
    //     }
    // }

}
