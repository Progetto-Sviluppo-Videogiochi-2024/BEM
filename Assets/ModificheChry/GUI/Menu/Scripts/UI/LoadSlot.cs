using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : SlotBaseManager
{
    public override void ListSlotUI()
    {
        var savedSlots = saveLoadSystem.dataService.ListSaves().OrderBy(slot => slot.Contains("Checkpoint") ? 1 : 0).ThenBy(slot => slot).ToList(); // Lista di tutti gli slot salvati dall'utente ordinati (es. "Slot 1", "Slot 2", ecc, e "Checkpoint")
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
                    ToggleSlotUI(currentUISlot, false);
                    LoadSlotUI(currentUISlot, savedSlotName);
                    savedSlotQueue.Dequeue();
                }
                else if (currentUISlot.name[^1] == 'C') ToggleSlotUI(currentUISlot, true); // Slot non corrispondente
                else ToggleSlotUI(currentUISlot, true); // Slot non corrispondente
            }
            else ToggleSlotUI(currentUISlot, true); // Non ci sono più salvataggi, configura lo slot come vuoto
        }
    }

    public override void OnYesActionSlot() => SaveLoadSystem.Instance.LoadGame(currentSlotOpened);

    protected override void OnYesDeleteSlot(Transform slot) // Per cancellare uno slot specifico
    {
        DeleteSlotUI(slot);
        ConfigureSlotButton(slot.GetComponent<Button>(), () => { }); // Disabilita il click
        SetMouseHoverSlot(slot, () => { }, () => { }); // Disabilita l'hover
        OnNoConfirmSlot();
    }

    protected override string GetConfirmMessage() =>
        isClickedOnDelete
            ? "Vuoi eliminare il salvataggio da questo slot ?"
            : $"Vuoi caricare il gioco da questo slot ?";
}
