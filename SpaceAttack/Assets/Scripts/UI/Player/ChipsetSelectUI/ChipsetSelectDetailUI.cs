using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipsetSelectDetailUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chipsetTitle;
    [SerializeField] private Image chipsetIcon;
    [SerializeField] private TextMeshProUGUI chipsetDescription;
    [SerializeField] private GameObject[] skills;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    [HideInInspector] public ChipsetSO currentChipset;
    [HideInInspector] public ChipsetDatabaseSO chipsetDatabase;

    private Image[] skillSprites;
    private Button[] buttons;
    void OnEnable()
    {
        SetChipsetDetail();
    }

    void OnDisable()
    {
        
    }

    private void SetChipsetDetail()
    {
        List<Image> i_temp = new List<Image>();
        List<Button> b_temp = new List<Button>();
        foreach (var skill in skills)
        {
            i_temp.Add(skill.GetComponent<Image>());
            b_temp.Add(skill.GetComponent<Button>());
        }
        skillSprites = i_temp.ToArray();
        buttons = b_temp.ToArray();

        string stringTemp = "";
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 0)
            {
                stringTemp = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.BaseAttack).description;
            }
            else if (i == 1)
            {
                stringTemp = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.Skill1).description;
            }
            else if (i == 2)
            {
                stringTemp = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.Skill2).description;
            }
            else if (i == 3)
            {
                stringTemp = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.Skill3).description;
            }

            buttons[i].onClick.AddListener(() =>
            {
                skillDescriptionText.text = stringTemp;
            });
        }
    }
}
