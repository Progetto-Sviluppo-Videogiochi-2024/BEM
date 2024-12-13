using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : SlotBaseManager
{
    public override void ListSlotUI()
    {
        var savedSlots = saveLoadSystem.dataService.ListSaves();
        var slotList = content.Cast<Transform>().ToList();
        var savedSlotQueue = new Queue<string>(savedSlots);

        for (int i = 0; i < slotList.Count; i++)
        {
            var currentUISlot = slotList[i];

            // Se ci sono ancora salvataggi disponibili
            if (savedSlotQueue.Count > 0)
            {
                var savedSlotName = savedSlotQueue.Peek();

                if (saveLoadSystem.dataService.SearchSlotFileByUI(savedSlotName, currentUISlot.name))
                {
                    ToggleSlotUI(currentUISlot, false);
                    LoadSlotUI(currentUISlot, savedSlotName);
                    savedSlotQueue.Dequeue();
                }
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
