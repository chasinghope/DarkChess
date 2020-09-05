using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour
{
    public Button Return;

    private void Start()
    {
        Return.onClick.AddListener(() =>{ this.gameObject.SetActive(false); GameManager.Instance.PlayButtonClickSound(); });
    }

}
