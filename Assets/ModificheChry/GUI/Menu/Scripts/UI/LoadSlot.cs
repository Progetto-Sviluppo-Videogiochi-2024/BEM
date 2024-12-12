using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadSlot : SlotBaseManager
{
    protected override void LoadSlotUI(Transform slot, string savedSlotName)
    {
        var gameData = saveLoadSystem.dataService.Load(savedSlotName);
        var slotSavedList = slot.Find("Saved");

        slotSavedList.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = $"Slot {gameData.nSlotSave}"; // "Slot i + 1"
        slotSavedList.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = gestoreScena.GetChapterBySceneName(gameData.currentSceneName); // "Capitolo: Nome Capitolo"
        slotSavedList.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = gameData.saveTime.ToString(); // "Data e Ora"

        ConfigureSlotButton(slotSavedList.GetComponentInChildren<Button>(), () => DeleteSlotUI(slot));
        ConfigureSlotButton(slot.GetComponent<Button>(), () => OpenConfirmPanel(savedSlotName));
        SetMouseHoverSlot(slot);
    }

    public override void OnYesActionSlot() => SaveLoadSystem.Instance.LoadGame(currentSlotOpened);

    protected override string GetConfirmMessage() => $"Vuoi caricare il gioco da questo slot ?";
}
