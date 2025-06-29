using UnityEngine;
using TMPro;

public class CopyWalletButton : MonoBehaviour
    {
    public TMP_Text walletAddressText;
    public TMP_Text publicKeyText;
    public TMP_Text privateKeyText;
    public TMP_Text mnemonicsText;

    // indexValue: 0 = address, 1 = publicKey, 2 = privateKey, 3 = mnemonics
    public void CopyValueByIndex(int indexValue)
        {
        string valueToCopy = "";
        string label = "";

        switch (indexValue)
            {
            case 0:
                valueToCopy = walletAddressText?.text;
                label = "Wallet Address";
                break;
            case 1:
                valueToCopy = publicKeyText?.text;
                label = "Public Key";
                break;
            case 2:
                valueToCopy = privateKeyText?.text;
                label = "Private Key";
                break;
            case 3:
                valueToCopy = mnemonicsText?.text;
                label = "Mnemonics";
                break;
            default:
                Debug.LogWarning("❌ Invalid index value.");
                return;
            }

        if (!string.IsNullOrEmpty(valueToCopy))
            {
            GUIUtility.systemCopyBuffer = valueToCopy;
            Debug.Log($"✅ Copied {label} to clipboard: {valueToCopy}");
            }
        else
            {
            Debug.LogWarning($"❌ {label} is empty or not assigned.");
            }
        }
    }
