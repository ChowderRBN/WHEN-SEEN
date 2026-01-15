using UnityEngine;

public class EchoPulse : MonoBehaviour
{
    public Material echoMat;

    public float pulseRadius = 0f;
    public float pulseLerpSpeed = 3f;

    void Update()
    {
        float movement = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        ).magnitude;

        bool crouching = Input.GetKey(KeyCode.LeftControl);

        float targetRadius =
            movement < 0.1f ? 0.5f :
            crouching ? 3f :
            10f;

        pulseRadius = Mathf.Lerp(
            pulseRadius,
            targetRadius,
            Time.deltaTime * pulseLerpSpeed
        );

        echoMat.SetVector("_Center", transform.position);
        echoMat.SetFloat("_Radius", pulseRadius);
    }
}
