using UnityEngine;
using System.Collections;
public class Collectable : MonoBehaviour
{
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        StartCoroutine(loadBlueprintSprite());

    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator loadBlueprintSprite()
    {
        while (Level.CurrentLevel == null)
        {
            yield return null;
        }
        GetComponent<SpriteRenderer>().sprite = Level.CurrentLevel.BlueprintSprite;
        if (Level.CurrentLevel.hasCollectedBlueprint)
        {
            gameObject.SetActive(false);
        }
    }

    public void pickUp()
    {
        Animator ani = GetComponent<Animator>();
        if (ani != null)
        {
            ani.enabled = true;
        }

        Level.CurrentLevel.hasCollectedBlueprint = true;
    }
    
}
