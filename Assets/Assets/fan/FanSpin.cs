using UnityEngine;

public class FanSpin : MonoBehaviour
{
    [SerializeField]
    private float fanSpeed = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, fanSpeed * Time.deltaTime);
    }
}
