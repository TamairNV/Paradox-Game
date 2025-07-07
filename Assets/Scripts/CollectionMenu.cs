using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CollectionMenu : MonoBehaviour
{
    [SerializeField] private Material blackOutMat;
    [SerializeField] private Material visibleMat;
    public List<Image> collectionItemSlots = new List<Image>();
    public List<Image> blueprintPieces = new List<Image>();
    public List<Transform> stars = new List<Transform>();
    [SerializeField] private Transform itemParent;
    [SerializeField] private Transform starsParent;
    [SerializeField] private List<Transform> blueprintParents = new List<Transform>(3);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (var child in itemParent.GetComponentsInChildren<Image>())
        {
            if (child.transform.parent == itemParent.transform)
            {
                collectionItemSlots.Add(child);
            }
            
        }

        foreach (var child in starsParent.GetComponentsInChildren<Transform>())
        {
            if (child.transform.parent == starsParent.transform)
            {
                stars.Add(child);
                child.gameObject.SetActive(false);
            }

        }
        foreach (var parent in blueprintParents)
        {
            foreach (var child in parent.GetComponentsInChildren<Image>())
            {
                blueprintPieces.Add(child);
            }
        }


        StartCoroutine(setVisibility());
    }

    IEnumerator setVisibility()
    {
        yield return null;
        int i = 0;
        foreach (var levelNumber in LevelPorter.levelPorters.Keys)
        {
            
            collectionItemSlots[levelNumber - 1].enabled = true;
            collectionItemSlots[levelNumber-1].sprite = LevelPorter.levelPorters[levelNumber].FinishItem.GetComponent<SpriteRenderer>().sprite;
            collectionItemSlots[levelNumber-1].material = blackOutMat;
            i++;
        }

        for (int j = i; j < collectionItemSlots.Count; j++)
        {
            collectionItemSlots[j].enabled = false;
        }
        foreach (var piece in blueprintPieces)
        {
            piece.material = blackOutMat;
        }

        
        foreach (var data in LevelSaveData.SaveData)
        {
            
            if (data.completed)
            {
                
                if (!data.addedToBook)
                {
                    yield return new WaitForSeconds(0.4f);
                    collectionItemSlots[data.levelNumber - 1].transform.GetChild(0).GetComponent<Image>().sprite =
                        collectionItemSlots[data.levelNumber - 1].sprite;
                    collectionItemSlots[data.levelNumber - 1].transform.GetChild(0).GetComponent<Image>().material =
                        visibleMat;
                    collectionItemSlots[data.levelNumber - 1].transform.GetChild(0).GetComponent<Animator>().Play("AddBook");
                    yield return new WaitForSeconds(0.5f);
                    data.addedToBook = true;
                }
                collectionItemSlots[data.levelNumber - 1].material = visibleMat;
                

                if (data.EntropyAtEnd <= LevelPorter.levelPorters[data.levelNumber].TargetEntropy )
                {
                    stars[data.levelNumber - 1].gameObject.SetActive(true);
                    if (!data.starAddedToBook)
                    {
                        stars[data.levelNumber - 1].transform.GetChild(0).GetComponent<Animator>().Play("AddStar");
                        yield return new WaitForSeconds(0.5f); 
                        data.starAddedToBook = true;
                    }
                    
                    
                    
                    
                    
                }

   
                foreach (var piece in blueprintPieces)
                {
                    if (piece.sprite == LevelPorter.levelPorters[data.levelNumber].Blueprint
                            .GetComponent<SpriteRenderer>().sprite)
                    {
                        if (data.hasCollectedBlueprint)
                        {
                            piece.material = visibleMat;
                        }
                        
                        break;
                    }
                }
            }

        }
        LevelSaveData.SaveAllData();
    }

    private void OnEnable()
    {
        StartCoroutine(setVisibility());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
