using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour {


    [SerializeField] float verticalOffset = 2f;
    [SerializeField] Slider armorSlider;
    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject healthFill;
    [SerializeField] GameObject armorFill;
    [SerializeField] Image HealthBackground;
    [SerializeField] Text playerName;
    [SerializeField] Color playerColor;
    [SerializeField] Color enemyColor;
    [SerializeField] Color playerRedColor;
    [SerializeField] Color redTeamColor;
    [SerializeField] Color playerBlueColor;
    [SerializeField] Color blueTeamColor;  
       
    private Transform target;

    private void Update()
    {
        if (target != null) {
            this.transform.position = new Vector2(target.position.x, target.position.y + verticalOffset);
        }
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    public void SetHealth(int health) {
        healthSlider.value = health;
        if (health <= 0)
        {
            healthFill.SetActive(false);
        }
        else {
            healthFill.SetActive(true);
        }
    }

    public void SetArmor(int armor) {        
        if (armor <= 0)
        {
            armorSlider.gameObject.SetActive(false);
        }
        else
        {
            armorSlider.gameObject.SetActive(true);
        }
        armorSlider.value = armor;
    }

    public void SetPlayerName(string playerName) {
        this.playerName.text = playerName;
    }
    

    public void SetPlayerColor(GameType gameType = GameType.FFA, Team team = Team.RED) {
        if (gameType == GameType.FFA)
        {
            healthFill.GetComponent<Image>().color = playerColor;
        }
        else {
            if(team == Team.RED)
                healthFill.GetComponent<Image>().color = playerRedColor;
            else
                healthFill.GetComponent<Image>().color = playerBlueColor;
        }
    }
    
    public void SetOtherPlayerColor(GameType gameType = GameType.FFA, Team team = Team.RED) {
        if (gameType == GameType.FFA)
        {
            healthFill.GetComponent<Image>().color = enemyColor;
        }
        else
        {
            if (team == Team.RED)
                healthFill.GetComponent<Image>().color = redTeamColor;
            else
                healthFill.GetComponent<Image>().color = blueTeamColor;
        }
    }

}
