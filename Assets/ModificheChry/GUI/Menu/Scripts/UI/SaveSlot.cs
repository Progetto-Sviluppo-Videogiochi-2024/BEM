using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;

public class SaveSlot : SlotBaseManager
{
    public override void ListSlotUI()
    {
        var savedSlots = saveLoadSystem.dataService.ListSaves().OrderBy(fileName => fileName.Contains("Checkpoint") ? 1 : 0).ThenBy(fileName => fileName).ToList(); // Lista di tutti gli slot salvati dall'utente ordinati (es. "Slot 1", "Slot 2", ecc, e "Checkpoint")
        var slotList = content.Cast<Transform>().ToList(); // Lista degli slot UI del canvas (es. Slot 1, Slot 2, ecc, e Checkpoint)
        var savedSlotQueue = new Queue<string>(savedSlots); // Coda degli slot salvati dall'utente (es. "Slot 1", "Slot 2", ecc, e "Checkpoint")

        for (int i = 0; i < slotList.Count; i++)
        {
            var currentUISlot = slotList[i]; // Slot UI corrente (es. Slot 1, Slot 2, ecc, e Checkpoint)
            if (savedSlotQueue.Count > 0)
            {
                var savedSlotName = savedSlotQueue.Peek(); // Prende il primo slot dalla coda (es. "Slot 1", "Slot 2", ecc, e "Checkpoint")
                if (saveLoadSystem.dataService.SearchSlotSaved(savedSlotName, currentUISlot.name))
                {
                    if (currentUISlot.name[0] == 'C') currentUISlot.GetComponent<Button>().interactable = false;
                    ToggleSlotUI(currentUISlot, false);
                    LoadSlotUI(currentUISlot, savedSlotName);
                    savedSlotQueue.Dequeue();
                }
                else // Slot non corrispondente
                {
                    ToggleSlotUI(currentUISlot, true);
                    ConfigureEmptySlot(currentUISlot);
                }
            }
            else // Non ci sono più salvataggi, configura lo slot come vuoto
            {
                ToggleSlotUI(currentUISlot, true);
                ConfigureEmptySlot(currentUISlot);
            }
        }
    }

    public override void OnYesActionSlot()
    {
        saveLoadSystem.SaveGame(currentSlotOpened == "C" ? 'C' : int.Parse(currentSlotOpened[^1].ToString())); // Ultimo carattere alfanumerico
        ListSlotUI();
    }


    protected override void OnYesDeleteSlot(Transform slot) // Per cancellare uno slot specifico
    {
        DeleteSlotUI(slot);
        ConfigureEmptySlot(slot);
        OnNoConfirmSlot();
    }

    protected override string GetConfirmMessage() =>
        isClickedOnDelete
            ? $"Vuoi eliminare il salvataggio su questo slot ?"
            : saveLoadSystem.dataService.DoesSaveExist(currentSlotOpened)
                ? $"Vuoi sovrascrivere il salvataggio su questo slot ?"
                : $"Vuoi salvare il gioco su questo slot ?";
}
