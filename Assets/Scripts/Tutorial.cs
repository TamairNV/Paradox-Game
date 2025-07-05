using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private List<Transform> stages = new List<Transform>();

    [SerializeField] private Canvas canvas;
    

    public int currentStage = 0;

    private bool changingPaper = false;

    public float yMovement = 120;

    public float angleRange = 5;

    public float speed = 50;

    public float rotationSpeed = 10;
    public float hideBuffer = 100;
    private Player player;

    private Vector2 startPosition;

    private bool isHidden = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        startPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    private TutorialData data;
    // Update is called once per frame
    void Update()
    {
        if (player.playingTutorial)
        {
            if (stages[currentStage].GetSiblingIndex() != transform.childCount-1 && !changingPaper)
            {
                StartCoroutine(ChangePaper(stages[currentStage]));
            }

            if (currentStage <= 2)
            {
                player.allowedToWalk = false;
            }

            if (isHidden)
            {
                player.allowedToWalk = true;
            }

            if (data == null)
            {
                data = GameObject.Find("Level").GetComponent<TutorialData>();
                player.transform.position = data.StartPosition.position;
            }
            
        }
        
        



    }

    private IEnumerator ChangePaper(Transform stage)
    {
        changingPaper = true;
        RectTransform stageRect = stage.GetComponent<RectTransform>();
        Vector3 startPos = stageRect.anchoredPosition;
        Vector3 offset = new Vector3(0, stageRect.sizeDelta.y, 0);
        Vector3 targetWorldPos = stage.parent.TransformPoint(startPos + offset);
        Quaternion targetRotation = Quaternion.Euler(0, 0, Random.Range(-angleRange, angleRange));

        while (Vector3.Distance(targetWorldPos, stage.position) > 0.07f)
        {
            stage.position = Vector3.Lerp(stage.position, targetWorldPos, speed * Time.deltaTime);
            yield return null;
        }

        stage.SetAsLastSibling();
        yield return new WaitForSeconds(0.1f);
        while (Vector3.Distance(stage.parent.TransformPoint(startPos), stage.position) > 0.01f)
        {
            Vector3 dynamicStartWorldPos = stage.parent.TransformPoint(startPos);
            stage.position = Vector3.Lerp(stage.position, dynamicStartWorldPos, speed * Time.deltaTime);
            stage.rotation = Quaternion.Lerp(stage.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }

        stageRect.anchoredPosition = startPos;
        changingPaper = false;
    }

    private IEnumerator HidePaperAnimation()
    {
        changingPaper = true;
        isHidden = true;
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        Vector2 targetPos = new Vector2(rectTransform.anchoredPosition.x ,canvas.GetComponent<RectTransform>().rect.yMax +transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y/2 - hideBuffer);
        while (Vector3.Distance(targetPos, rectTransform.anchoredPosition) > 0.07f)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos;
        
        changingPaper = false;
    }

    public void HidePaper()
    {
        if (changingPaper)
        {
            return;
        }
        StartCoroutine(HidePaperAnimation());
    }

    private IEnumerator ShowPaperAnimation()
    {
        changingPaper = true;
        isHidden = false;
        RectTransform rectTransform = transform.GetComponent<RectTransform>();

        while (Vector3.Distance(startPosition, rectTransform.anchoredPosition) > 0.07f)
        {
            rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, startPosition, speed * Time.deltaTime);
            yield return null;
        }

        rectTransform.anchoredPosition = startPosition;
        changingPaper = false;
    }

    public void ShowPaper()
    {
        if (changingPaper)
        {
            return;
        }
        StartCoroutine(ShowPaperAnimation());
    }


    public void IncreaseStageBy(int amount)
    {
        if (changingPaper)
        {
            return;
        }

        currentStage += amount;
    }
    public void DecreaseStageBy(int amount)
    {
        if (changingPaper)
        {
            return;
        }
        currentStage -= amount;
    }
    
    
}
