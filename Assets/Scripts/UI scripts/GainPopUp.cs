using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GainPopUp : MonoBehaviour
{
    private static GameObject gainPopUpPrefab;
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] SpriteRenderer spriteRdr;
    private Color textColor;
    private Color spriteColor;
    private float disappearTimer;

    private void Awake()
    {
        if (!textMesh) textMesh = GetComponentInChildren<TextMeshPro>();
        if (!spriteRdr) spriteRdr = GetComponentInChildren<SpriteRenderer>();
        textColor = textMesh.color;
        spriteColor = spriteRdr.color;
    }

    public static GainPopUp Create(Vector3 worldPosition, int amount, ResourceType resource)
    {
        gainPopUpPrefab = Resources.Load("GainPopUp") as GameObject;
        GameObject gainPopUpObject = Instantiate(gainPopUpPrefab, worldPosition, Quaternion.identity);
        GainPopUp gainPopUp = gainPopUpObject.GetComponent<GainPopUp>();
        gainPopUp.Setup(amount, resource);

        return gainPopUp;
    }

    public void Setup(int amount, ResourceType resource)
    {
        textMesh.text = "+" + amount;
        disappearTimer = 1f;

        switch(resource)
        {
            case ResourceType.Food:
                spriteRdr.sprite = GameAssets.i.foodIcon;
                break;
            case ResourceType.Wood:
                spriteRdr.sprite = GameAssets.i.woodIcon;
                break;
            case ResourceType.Stone:
                spriteRdr.sprite = GameAssets.i.stoneIcon;
                break;
        }
    }

    private void Update()
    {
        disappearTimer -= Time.deltaTime;
        float moveYSpeed = 5;
        transform.position += new Vector3(0, moveYSpeed, 0) * Time.deltaTime;

        if (disappearTimer < 0)
        {
            float disappearSpeed = 5f;
            Debug.Log("sprite alpha " + spriteColor.a);
            textColor.a -= disappearSpeed * Time.deltaTime;
            spriteColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            spriteRdr.color = spriteColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
