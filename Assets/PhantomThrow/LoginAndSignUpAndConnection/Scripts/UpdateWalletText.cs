using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateWalletText : MonoBehaviour
{

    public TextMeshProUGUI walletAddressValue;
    public TextMeshProUGUI usernameValue;
    public TextMeshProUGUI mnemonicsValue;

    // Start is called before the first frame update
    void Start()
    {
        usernameValue.text = UserSession.Instance.Username;
        walletAddressValue.text = UserSession.Instance.WalletAddress;
        mnemonicsValue.text = UserSession.Instance.Mnemonics;
        }
}
