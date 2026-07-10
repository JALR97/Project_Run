using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
    //Balance
    [SerializeField] private float maxSips = 3;
    [SerializeField] private float staminaRegen = 15;
    [SerializeField] private float sipCooldown = 1;
    
    //Componentes
    [SerializeField] private TMP_Text waterText;
    [SerializeField] private TMP_Text waterCooldownText;
    [SerializeField] private Button waterButton;
    [SerializeField] private Button shoeButton;
    [SerializeField] private ResourceEngine resourceEngine;
    
    //Inner work
    private int Sips = 3;
    private float waterTimer;
    private bool inCooldown;

    public void ShoeEffect() {
        resourceEngine.Nitro();
        shoeButton.interactable = false;
    }

    private void Start() {
        waterText.text = $"Water bottle\n{maxSips}/{maxSips} sips";
    }

    public void TakeSip() {
        if (Sips > 0 && !inCooldown) {
            Sips--;
            inCooldown = Sips > 0;
            waterButton.interactable = false;
            waterText.text = $"Water bottle\n{Sips}/{maxSips} sips";
            resourceEngine.WaterBoost();
        }
    }

    private void Update() {
        if (inCooldown) {
            waterTimer += Time.deltaTime;
            if (waterTimer >= sipCooldown) {
                waterTimer = 0f;
                inCooldown = false;
                waterButton.interactable = true;
                waterCooldownText.text = "";
            }
            else {
                waterCooldownText.text = "cooldown: "+(sipCooldown - waterTimer).ToString("0");
            }
        }
    }
}
