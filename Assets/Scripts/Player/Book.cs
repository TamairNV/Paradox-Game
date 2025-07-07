using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class Book : MonoBehaviour
{
    private Player player;


    [SerializeField] public Image BlueprintImage;
    [SerializeField] public Image CollecableImage;
    [SerializeField] public TMP_Text TargetEntropy;
    public bool bookOpen = false;
    private bool isDoingThings = false;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }   
    public void RunCloseBook()
    {
        if (isDoingThings)
        {
            return;
        }
        StartCoroutine(closeBook());
    }
    public void ToggleProgress()
    {
        if (isDoingThings)
        {
            return;
        }
        if (bookOpen)
        {
            RunCloseBook();
           
        }
        else
        {
            StartCoroutine(openBook("progress"));
            
        }
        
        
    }
    public void OpenSettings()
    {
        if (isDoingThings)
        {
            return;
        }
        StartCoroutine(openSettings());
    }

    public void TurnBackToProgess()
    {
        if (isDoingThings)
        {
            return;
        }
        StartCoroutine(turnBackToProgress());
    }
    public IEnumerator closeBook()
    {
        isDoingThings = true;
        RectTransform bookRect = GetComponent<RectTransform>();
        Animator bookAnimator = GetComponent<Animator>();
    
        bookAnimator.Play("bookclose");
        yield return new WaitForSeconds(0.2f);
  
        transform.GetChild(0).gameObject.SetActive(false);
        
        transform.GetChild(1).gameObject.SetActive(false);
        
        transform.GetChild(2).gameObject.SetActive(false);
        
        yield return new WaitForSeconds(0.4f);
    
        Vector2 startAnchoredPos = bookRect.anchoredPosition;
        float speed = 5000;
        float t = 0;
    
        while (t < 0.6f)
        {
            bookRect.anchoredPosition += Vector2.down * speed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
    
        GetComponent<Image>().enabled = false;
        bookRect.anchoredPosition = startAnchoredPos;
        bookOpen = false;
        yield return new WaitForSeconds(0.6f);
        isDoingThings = false;
        
    }

    public IEnumerator openBook(string page)
    {
        bookOpen = true;
        isDoingThings = true;
        RectTransform bookRect = GetComponent<RectTransform>();
        Animator bookAnimator = GetComponent<Animator>();

       
        Vector2 startAnchoredPos = bookRect.anchoredPosition;

 
        float offset = 1000f; 
        bookRect.anchoredPosition = startAnchoredPos + Vector2.down * offset;
        GetComponent<Image>().enabled = true;

     
        float duration = 0.5f; 
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); 
            bookRect.anchoredPosition = Vector2.Lerp(
                startAnchoredPos + Vector2.down * offset, 
                startAnchoredPos, 
                t
            );
            yield return null;
        }


        bookRect.anchoredPosition = startAnchoredPos;

     
        bookAnimator.Play("bookopen");
        yield return new WaitForSeconds(0.2f);


        if (page.Equals("levelStart"))
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else if (page.Equals("settings"))
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (page.Equals("progress"))
        {
            transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Unknown page: " + page);
        }
        yield return new WaitForSeconds(0.6f);
        isDoingThings = false;
    }
    private IEnumerator openSettings()
    {
        isDoingThings = true;
        Animator bookAnimator = GetComponent<Animator>();
        GetComponent<Image>().enabled = true;
        
        bookAnimator.Play("turnPageForward");
        yield return new WaitForSeconds(0.15f);
        transform.GetChild(2).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.6f);
        isDoingThings = false;
    }
    
    private IEnumerator turnBackToProgress()
    {
        isDoingThings = true;
        Animator bookAnimator = GetComponent<Animator>();
        GetComponent<Image>().enabled = true;
        
        bookAnimator.Play("turnPageBackwards");
        yield return new WaitForSeconds(0.15f);
        transform.GetChild(1).gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.6f);
        isDoingThings = false;
    }

}
