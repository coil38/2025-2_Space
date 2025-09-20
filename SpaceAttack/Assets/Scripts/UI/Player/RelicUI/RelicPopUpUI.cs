using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class RelicPopUpUI : MonoBehaviour
{
    [Header("팝업UI정보전달 대상들")]
    [SerializeField] private TextMeshProUGUI relicNameText;
    [SerializeField] private TextMeshProUGUI relicDivisionText;
    [SerializeField] private TextMeshProUGUI relicDarkMatCountText;
    [SerializeField] private TextMeshProUGUI relicAbilityText;
    [SerializeField] private TextMeshProUGUI relicDescription;
    [SerializeField] private Image relicIconImage;

    public void SetRelicPopUpUI(bool onRelicPopUp, BaseRelic relic = null)
    {
        if (onRelicPopUp)
        {
            gameObject.SetActive(true);
            LogUtil.Log("유물팝업UI 활성화");
            Vector3 targetPos = relic.transform.position + relic.transform.up * 0.6f;
            transform.position = targetPos;

            RelicDatabaseSO database = DataManager.instance._RelicDatabase;
            //유물 정보 입력
            RelicSO relicSO = database.GetRelic(relic.relicId);                 //유물SO찾기
            relicNameText.text = relicSO.relicName;                             //유물 이름할당
            relicDivisionText.text = relicSO.relicDivision;                     //유물 분류 할당
            relicDarkMatCountText.text = relicSO.darkMaterialCount.ToString();  //유물 암흑물질수 할당
            relicDescription.text = relicSO.description;                        //유물 설명 할당
            relicIconImage.sprite = relicSO.iconSprite;                         //유물 이미지 할당

            //유물 능력 할당
            List<string> temp = new List<string>();
            string[] words;

            foreach (var effectId in relicSO.relicEffects)
            {
                string des = database.relicEffectDatabase.GetRelicEffect(effectId).relicEffectDiscription;
                words = des.Replace(".", string.Empty).Split(' ');
                string sentence = "";
                RelicInfo relicInfo = GetRelicInfo(effectId, relicSO);

                for (int i = 0; i < words.Length; i++)
                {
                    if (words[i] == "n")
                    {
                        words[i] = relicInfo.n.ToString();
                    }
                    else if (words[i] == "n%")
                    {
                        words[i] = relicInfo.n.ToString() + "%";
                    }
                    else if (words[i] == "z")
                    {
                        words[i] = relicInfo.z.ToString();
                    }
                    //LogUtil.Log(words[i]);
                    sentence += " " + words[i];
                }
                temp.Add(sentence);
            }

            relicAbilityText.text = string.Join("\n", temp);
        }
        else
        {
            gameObject.SetActive(false);
            LogUtil.Log("유물팝업UI 비활성화");
        }
    }

    private RelicInfo GetRelicInfo(int id, RelicSO relic)
    {
        foreach (var info in relic.relicInfos)
        {
            if (id == info.id)
            {
                return info;
            }
        }
        return null;
    }
}
