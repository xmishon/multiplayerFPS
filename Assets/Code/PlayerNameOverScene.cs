using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameOverScene : MonoBehaviour
{
    public string PlayerName;

    [SerializeField] private Text _text;
    [SerializeField] private Behaviour _behaviourToEnable;

    public void SetPlayerName()
    {
        PlayerName = _text.text;
        _behaviourToEnable.enabled = true;
    }
}
