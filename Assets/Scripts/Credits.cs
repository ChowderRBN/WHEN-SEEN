using UnityEngine;
using TMPro;

public class Credits : MonoBehaviour
{
    public TextMeshProUGUI creditsText;

    void Start()
    {
        creditsText.text =
            "When Seen\n\n" +
            "Created By\n" +
            "ChowderRBN\n\n" +
            "—\n\n" +
            "Cave System\n"+ 
            "Mine\n\n" +
            "Gregory Seguru\n\n" +
            "—\n\n" +
            "Environment Models\n" +
            "Evgenia\n\n" +
            "Asset Pack:\n" +
            "Lowpoly Piece of Nature\n\n" +
            "—\n\n" +
            "Audio\n" +
            "Horror Elements\n" +
            "By Anthon\n\n" +
            "—\n\n" +
            "In Loving Memory\n\n" +
            "Richard Bryan \"Pa\" Notley\n\n" +
            "Kathleen \"Kathy\" \"Nanny\" Notley\n\n" +
            "Rest in Peace";
    }
}
