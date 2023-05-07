using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logic : MonoBehaviour
{
    public Text test;
    public QRCodeDecodeController qrcodecontroller;
    public float qrResetTimer;
    public float qrCooldown;
    public bool hasScanned;
    public string effect;
    public string currentCard;
    public bool scannedOnce;
    public List<string> usedCards;
    public string cueCommand;

    [SerializeField] OscOut _oscOut;
    // Start is called before the first frame update
    void Start()
    {
        //Opens OSC port and creates a new list of used cards.
        usedCards = new List<string>();
        _oscOut.Open(53000, "10.0.0.5");
        scannedOnce = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Loop to check if a QR code has been detected, and if the redemption is off cooldown.
        if (qrResetTimer <= 0 && hasScanned)
        {
            qrcodecontroller.Reset();
            hasScanned = false;
            scannedOnce = false;
        }
        //Rejects the QR code if it has already been scanned.
        else if (qrResetTimer <= 0 && !scannedOnce)
        {
            qrcodecontroller.onQRScanFinished += getResult;
            scannedOnce = true;
        }
        //Ticks the cooldown timer down
        else
        {
            qrResetTimer -= Time.deltaTime;
        }
    }

    //Checks the information of the scanned QR code.
    void getResult(string currentCard)
    {
        //Checks if the current card is in the usedCards list.
        if (usedCards.Contains(currentCard))
        {
            //test.text = "sorry"; Debug code
            hasScanned = true;
        }
        //Sends and OSC command of /cue/{Name of Effect}/go to the designated IP address, running QLab, to trigger events based on the scanned card, then marks the card as scanned.
        else
        {
           //test.text = "yee"; Debug code
            usedCards.Add(currentCard);
            effect = currentCard.Split('_')[0];       
            cueCommand = "/cue/" + effect + "/go";
            Debug.Log(cueCommand);
            _oscOut.Send(cueCommand);
            hasScanned = true;
            qrResetTimer = qrCooldown;
        }
    }
}
