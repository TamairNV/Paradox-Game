using UnityEngine;

public class Book : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject book;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
