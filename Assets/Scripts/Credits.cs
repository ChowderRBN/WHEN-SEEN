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
             "Environment Models  \r\nEvgenia  \r\n\r\nAsset Pack: Lowpoly Piece of Nature\r\n" +
             "—\n\n" +
            "In Loving Memory\n\n" +
            "Richard Bryan \"Pa\" Notley\n" +
            " \n\n" +
            "Kathleen \"Kathy\" \"Nanny\" Notley\n" +
            "Rest in Peace";
    }
}
