using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    protected override void LoadSlotUI(Transform slot, string savedSlotName)
    {
        var gameData = saveLoadSystem.dataService.Load(savedSlotName);
        var slotSavedList = slot.Find("Saved");

        slotSavedList.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"Slot {gameData.nSlotSave}"; // "Slot nSlotSave"
        slotSavedList.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = gestoreScena.GetChapterBySceneName(gameData.currentSceneName); // "Capitolo: Nome Capitolo"
        slotSavedList.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = gameData.saveTime.ToString(); // "Data e Ora"

        ConfigureSlotButton(slotSavedList.GetComponentInChildren<Button>(), () => { DeleteSlotUI(slot); ConfigureEmptySlot(slot); });
        ConfigureSlotButton(slot.GetComponent<Button>(), () => OpenConfirmPanel(savedSlotName));
        SetMouseHoverSlot(slot);
    }

    public override void OnYesActionSlot()
    {
        saveLoadSystem.SaveGame(int.Parse(currentSlotOpened[^1].ToString())); // Ultimo carattere
        ListSlotUI();
    }

    protected override string GetConfirmMessage() =>
        saveLoadSystem.dataService.DoesSaveExist(currentSlotOpened)
            ? $"Vuoi sovrascrivere il salvataggio su questo slot ?"
            : $"Vuoi salvare il gioco su questo slot ?";
}
