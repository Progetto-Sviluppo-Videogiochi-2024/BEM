using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    protected override void LoadSlotUI(Transform slot, string savedSlotName)
    {
        var gameData = saveLoadSystem.dataService.Load(savedSlotName);
        var slotSavedList = slot.Find("Saved");

        slotSavedList.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"Slot {gameData.nSlotSave}"; // "Slot i + 1"
        slotSavedList.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = gestoreScena.GetChapterBySceneName(gameData.currentSceneName); // "Capitolo: Nome Capitolo"
        slotSavedList.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = gameData.saveTime.ToString(); // "Data e Ora"

        ConfigureSlotButton(slotSavedList.GetComponentInChildren<Button>(), () => { DeleteSlotUI(slot); currentSlotOpened = null; slot.GetComponent<Button>().onClick.RemoveAllListeners(); });
        ConfigureSlotButton(slot.GetComponent<Button>(), () => OpenConfirmPanel(savedSlotName));
        SetMouseHoverSlot(slot);
    }

    public override void OnYesActionSlot() => SaveLoadSystem.Instance.LoadGame(currentSlotOpened);

    protected override string GetConfirmMessage() => $"Vuoi caricare il gioco da questo slot ?";
}
