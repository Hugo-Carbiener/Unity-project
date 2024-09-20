using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using DG.Tweening;

public class BattlefieldUIManager : UIManager, IActiveUI
{
    private static readonly string ALLY_TROOP_CONTAINER = "AlliesContainer";
    private static readonly string ENEMY_TROOP_CONTAINER = "EnemiesContainer";
    private static readonly string BACKGROUND_CONTAINER = "Background";
    private static readonly string BUILDING_SPRITE_CONTAINER = "BuildingSpriteContainer";
    private static readonly string TROOP_SPRITE_CONTAINER = "SpriteContainer";

    private static BattlefieldUIManager _instance;
    public static BattlefieldUIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BattlefieldUIManager>();
            }

            return _instance;
        }
    }

    [Header("Troop template")]
    [SerializeField] private VisualTreeAsset troopElement;
    [Header("Rendering")]
    [SerializeField] private int width;
    [SerializeField] private int groundHeight;
    [SerializeField] private int backgroundHeight;
    [SerializeField] private Canvas renderCanvas; 
    [SerializeField] private UnityEngine.UI.Image renderSupport;
    [Header("Visuals")]
    [SerializeField] private float troopApparitionFadeInDuration;
    [SerializeField] private float troopApparitionFadeOutDuration;
    [SerializeField] private float troopWidthFractionDisplacement;
    [SerializeField] private float attackDuration;
    [SerializeField] private float attackWindUpDuration;
    [SerializeField] private float damageIndicatorDelay;
    [SerializeField] private Color damageIndicatorColor;
    public Fight currentFight { get; set; }

    Sequence attackSequence;

    private void Awake()
    {
        root = document.rootVisualElement;
        Assert.IsNotNull(renderCanvas);
        Assert.IsNotNull(renderSupport);
    }

    void Start()
    {
        SetVisibility(DisplayStyle.None);
        renderSupport.gameObject.SetActive(false);
        
        PixelPerfectCamera pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        renderSupport.rectTransform.sizeDelta = new Vector2(pixelPerfectCamera.refResolutionX, pixelPerfectCamera.refResolutionY);
    }

    public bool CanBeOpened()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        return selectedCell != null
            && selectedCell.fight != null
            && !BookUIManager.Instance.IsVisible()
            && !BuildingUIManager.Instance.IsVisible();
    }

    public void CloseUIComponent()
    {
        renderSupport.gameObject.SetActive(false);
        SetVisibility(root, DisplayStyle.None);
        ResetUIComponent();
    }

    public void OpenUIComponent()
    {
        SetVisibility(DisplayStyle.Flex);
        InitUIComponent();
        renderSupport.gameObject.SetActive(true);
    }

    public void ResetUIComponent()
    {
        VisualElement alliesTroopContainer = root.Q<VisualElement>(ALLY_TROOP_CONTAINER);
        VisualElement enemiesTroopContainer = root.Q<VisualElement>(ENEMY_TROOP_CONTAINER);
        alliesTroopContainer.Clear();
        enemiesTroopContainer.Clear();
        MainMenuUIManager.Instance.UpdateUIComponent();
        attackSequence.Kill();
    }

    public void ToggleUIComponent()
    {
        if (IsVisible())
        {
            CloseUIComponent();
        }
        else
        {
            OpenUIComponent();
        }
    }

    private void InitUIComponent()
    {
        CellData selectedCell = TileSelectionManager.Instance.GetSelectedCellData();
        if (selectedCell == null)
        {
            Debug.LogError("Opened Battlefield with no tile selected.");
            return;
        }

        Fight fight = selectedCell.fight;
        if (fight == null)
        {
            Debug.LogError("Opened Battlefield with no fight on the selected tile.");
            return;
        }

        VisualElement allyTroopContainer = root.Q<VisualElement>(ALLY_TROOP_CONTAINER);
        VisualElement enemyTroopContainer = root.Q<VisualElement>(ENEMY_TROOP_CONTAINER);

        currentFight = fight;
        InitTroops(fight, Factions.Villagers, allyTroopContainer);
        InitTroops(fight, Factions.Monsters, enemyTroopContainer);
        InitBuilding(selectedCell);
        fight.OnFighterAdded.AddListener(AddTroop);
        currentFight.OnFightEndEvent.AddListener(CloseUIComponent);
        MainMenuUIManager.Instance.UpdateUIComponent();
    }

    private void InitTroops(Fight fight, Factions faction, VisualElement troopContainer)
    {
        int troopCounter = 0;
        foreach (FightModule fighter in fight.teams[faction].fighters)
        {
            TemplateContainer troopElementToAdd = InitTroopElement(fighter, faction == Factions.Villagers, troopCounter);
            if (troopElementToAdd == null) continue;

            troopContainer.Add(troopElementToAdd);
            VisualElement spriteContainer = troopElementToAdd.Q<VisualElement>(TROOP_SPRITE_CONTAINER);
            AddEventListeners(fighter, spriteContainer, false);
            troopCounter++;
        }
    }
    private void InitBuilding(CellData cell)
    {
        Building building = cell.building;
        if (building)
        {
            VisualElement buildingSpriteContainer = root.Q<VisualElement>(BUILDING_SPRITE_CONTAINER);
            if (buildingSpriteContainer == null)
            {
                Debug.LogError("Could not find building sprite container in building container of battlefield.");
                return;
            }
            buildingSpriteContainer.style.backgroundImage = new StyleBackground(GameAssets.i.buildingSprites[building.GetBuildingType()]);
            AddEventListeners(building.GetFightModule(), buildingSpriteContainer, true);
        }
    }

    private TemplateContainer InitTroopElement(FightModule fighter, bool isFacingRight, int troopCounter)
    {
        if (fighter.GetActor().IsBuilding()) return null; 
        
        TemplateContainer troopElementToAdd = troopElement.Instantiate();
        VisualElement spriteContainer = troopElementToAdd.Q<VisualElement>(TROOP_SPRITE_CONTAINER);
        if (spriteContainer == null)
        {
            Debug.LogError("Could not find troop sprite container in troop element of battlefield.");
            return null;
        }

        Sprite sprite = fighter.GetComponent<SpriteRenderer>().sprite;
        Vector2 spriteSize = sprite.rect.size;
        spriteContainer.style.backgroundImage = new StyleBackground(sprite);
        spriteContainer.style.width = spriteSize.x;
        spriteContainer.style.height = spriteSize.y;
        if (isFacingRight)
        {
            spriteContainer.transform.scale = spriteContainer.transform.scale + new Vector3(-2, 0, 0);
        }
        return troopElementToAdd;
    }

    private void AddEventListeners(FightModule fighter, VisualElement container, bool isBuilding)
    {
        if (isBuilding)
        {
            fighter.OnDamaged.AddListener(damageValue => OnDamaged(container, damageValue));
            fighter.OnDeath.AddListener(delegate { OnDeath(container); });
        } else
        {
            fighter.OnAttack.AddListener(target => OnAttack(container, target));
            fighter.OnDamaged.AddListener(damageValue => OnDamaged(container, damageValue));
            fighter.OnDeath.AddListener(delegate { OnDeath(container); });
            fighter.OnFlee.AddListener(delegate { OnFlee(container); });
        }
    }

    // EVENT ACTIONS

    /**
    * Method used to add a troop while the battlefield is already open.
    * When first opening the battlefield, it is generated with all currently present troops.
    */
    private void AddTroop(FightModule fighter)
    {
        if (!IsVisible()) return;

        VisualElement troopContainer = null;
        Factions faction = fighter.GetFaction();
        switch (faction)
        {
            case Factions.Villagers:
                troopContainer = root.Q<VisualElement>(ALLY_TROOP_CONTAINER);
                break;
            case Factions.Monsters:
                troopContainer = root.Q<VisualElement>(ENEMY_TROOP_CONTAINER);
                break;
        }
        TemplateContainer troopElementToAdd = InitTroopElement(fighter, faction == Factions.Villagers, troopContainer.childCount);
        if (troopElementToAdd == null) return;
        AddEventListeners(fighter, troopElementToAdd, false);
        troopContainer.Add(troopElementToAdd);
    }

    private IEnumerator RemoveTroop(VisualElement troopContainer)
    {
        float timer = 0;
        while (timer < troopApparitionFadeOutDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(255, 0, timer / troopApparitionFadeOutDuration);
            Color color = troopContainer.style.unityBackgroundImageTintColor.value;
            color.a = alpha;
            troopContainer.style.unityBackgroundImageTintColor = color;

            yield return null;
        }
        troopContainer.parent.Remove(troopContainer);
    }

    private void OnAttack(VisualElement troopContainer, FightModule target)
    {
        float startPos = troopContainer.transform.position.x;
        float windUpEndPos = troopContainer.transform.position.x;
        float attackEndPos = troopContainer.transform.position.x;
        float pos = startPos;

        if (target.GetFaction() != Factions.Villagers)
        {
            attackEndPos += troopWidthFractionDisplacement * troopContainer.layout.width;
            windUpEndPos -= troopWidthFractionDisplacement * troopContainer.layout.width / 2;
        } else
        {
            attackEndPos -= troopWidthFractionDisplacement * troopContainer.layout.width;
            windUpEndPos += troopWidthFractionDisplacement * troopContainer.layout.width / 2;
        }

       
        attackSequence = DOTween.Sequence();
        // wind up
        attackSequence.Append(DOTween.To(() => startPos, x => pos = x, windUpEndPos, attackWindUpDuration / 2)
            .OnUpdate(() =>
            {
                troopContainer.transform.position = new Vector2(pos, transform.position.y);
            })
            .SetEase(Ease.Linear));
        // forward step
        attackSequence.Append(DOTween.To(() => pos, x => pos = x, attackEndPos, attackDuration / 2)
                .OnUpdate(() =>
                {
                    troopContainer.transform.position = new Vector2(pos, troopContainer.transform.position.y);
                })
                .SetEase(Ease.InQuad));            
        // return 
        attackSequence.Append(DOTween.To(() => pos, x => pos = x, startPos, attackDuration / 2)
            .OnUpdate(() =>
            {
                troopContainer.transform.position = new Vector2(pos, troopContainer.transform.position.y);
            })
            .SetEase(Ease.OutQuad));
    }

    private void OnDamaged(VisualElement troopContainer, int damageValue)
    {
        StartCoroutine(damageFlashIndicator(30, troopContainer));

        string text = "-" + damageValue.ToString();
        VisualElement background = root.Q<VisualElement>(BACKGROUND_CONTAINER);
        Vector3 centerOffset = troopContainer.layout.center - troopContainer.layout.position;
        centerOffset = new Vector3(Mathf.Abs(centerOffset.x), Mathf.Abs(centerOffset.y));
        Vector3 anchor = troopContainer.worldTransform.GetPosition() + centerOffset;
        anchor = new Vector3(anchor.x, anchor.y - background.layout.height, 0);

        PopUpLauncher.LaunchPopUp(this, GameAssets.i.battleFieldPopUp, text, anchor);
    }

    private IEnumerator damageFlashIndicator(int frameDuration, VisualElement spriteContainer)
    {
        yield return new WaitForSeconds(damageIndicatorDelay);
        spriteContainer.style.unityBackgroundImageTintColor = new StyleColor(damageIndicatorColor);
        yield return frameDuration;
        spriteContainer.style.unityBackgroundImageTintColor = new StyleColor(Color.white);
    }

    private void OnDeath(VisualElement troopContainer)
    {
        StartCoroutine(RemoveTroop(troopContainer));
    }

    private void OnFlee(VisualElement troopContainer)
    {
        StartCoroutine(RemoveTroop(troopContainer));
    }

    public void UpdateVisibility() {}
}
