using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SaveSlot : SlotBaseManager
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
        saveLoadSystem.SaveGame(int.Parse(currentSlotOpened[^1].ToString())); // Ultimo carattere
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
