using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] private float capSizeAngle = 20f;
    private Player player;
    

    void Start()
    {
        player = FindFirstObjectByType<Player>();
    }

    void Update()
    {
        if(IsCapSized()){
            player.GameOver();

        }
    }

    private bool IsCapSized()
    {
        float xAngle = Mathf.Abs(NormalizeAngle(transform.eulerAngles.x));
        float zAngle = Mathf.Abs(NormalizeAngle(transform.eulerAngles.z));

        float averageTilt = (xAngle + zAngle) /2f;

        return averageTilt > capSizeAngle;
    }


    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }

}
