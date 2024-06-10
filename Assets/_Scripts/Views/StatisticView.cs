using TMPro;
using UnityEngine;

public class StatisticView : MonoBehaviour
{
    [SerializeField] PlayerStats stat;
    [SerializeField] TextMeshProUGUI TMP_Resource;

    private void OnEnable()
    {
        Player.GetInstance.GetPlayerData.OnDataChange += GetPlayerData;
    }

    private void Start()
    {
        GetPlayerData();
    }

    private void GetPlayerData()
    {
        PlayerStatView(stat, TMP_Resource);
    }

    private void PlayerStatView(PlayerStats stats, TextMeshProUGUI toUpdate)
    {
        if (Player.GetInstance.GetPlayerData.TryGet(out int value, stats))
        {
            toUpdate.text = value.ToString();
        }
        else
        {
            toUpdate.text = "0";
        }
    }

    private void OnDisable()
    {
        Player.GetInstance.GetPlayerData.OnDataChange -= GetPlayerData;
    }


}

