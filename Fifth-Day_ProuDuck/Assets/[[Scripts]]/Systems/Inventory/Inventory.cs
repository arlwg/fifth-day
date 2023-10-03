using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.PerformanceData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private static Inventory instance = null;
    public static Inventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Inventory>();
            }
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    private void Awake()
    {
        instance = this;
    }

    public Item selectItem = null;
    public ItemSlot selectItemSlot = null;

    //drag and drop
    // Reference: https://www.youtube.com/watch?v=FdxvTcHJiA8 by Dev Leonardo for drag and drop
    public bool isDragging;
    public GameObject dragObject;
    public ItemSlotList itemSlotList;
    public List<QuickSlotController> quickSlotList;

    private bool isInventoryPanelJustOpen;
    private Image dragIcon;
    private Camera camera;
    private Vector2 screenPosition;
    private Vector3 worldPosition;
    private bool isUsingMobileInput = false;
    private bool isClicking = false;
    private bool isHelding = false;
    private bool isOverlapWithItemSlot = false;
    private ItemSlot dropSelectItemSlot = null;
    public GameObject inventoryPanel;

    private void Start()
    {
        camera = Camera.main;

        isUsingMobileInput = Application.isMobilePlatform;

        var quickSlots = FindObjectsOfType<QuickSlotController>();

        foreach (var quickSlot in quickSlots)
        {
            quickSlotList.Add(quickSlot);
        }

        itemSlotList = FindObjectOfType<ItemSlotList>();
        inventoryPanel = itemSlotList.gameObject;
        isInventoryPanelJustOpen = false;

        dragIcon = dragObject.GetComponent<Image>();
        dragObject.SetActive(false);
    }

    public bool isSelectingItem()
    {
        return selectItem != null;
    }

    public void RemoveSelecting()
    {
        selectItem = null;
        selectItemSlot = null;
        dropSelectItemSlot = null;
    }

    public bool IsDragging()
    {
        return isDragging;
    }

    private void Update()
    {

        // catch the input position
        if (isUsingMobileInput)
        {
            if (Input.touchCount > 0)
            {
                screenPosition = Input.GetTouch(0).position;
                isClicking = true;
            }
            else
            {
                isClicking = false;
            }

            worldPosition = camera.ScreenToWorldPoint(screenPosition);
        }
        else
        {
            if(Input.GetMouseButton(0))
            {
                screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                isClicking = true;

            }
            else
            {
                isClicking = false;
            }

            worldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        }


        if (isClicking)
        {
            // Ray check
            PointerEventData pointerEventData;

            pointerEventData = new PointerEventData(FindObjectOfType<EventSystem>());
            pointerEventData.position = screenPosition;


            //Ray ray = Camera.main.ScreenPointToRay(screenPosition);

            List<RaycastResult> results = new List<RaycastResult>();
            //GraphicRaycaster graphicRaycaster = FindObjectOfType<GraphicRaycaster>();

            //graphicRaycaster.Raycast(pointerEventData, results);
            EventSystem.current.RaycastAll(pointerEventData, results);

            //RaycastHit hit;
            //Physics.Raycast(ray, out hit);

            isOverlapWithItemSlot = false;

            if (isDragging)
            {
                dragObject.transform.position = Vector2.Lerp(dragObject.transform.position, screenPosition, Time.deltaTime * 10.0f);

                // find if is pointing to the itemslot to drop
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject!= null)
                    {
                        ItemSlot item = result.gameObject.GetComponent<ItemSlot>();

                        if (item != null)
                        {
                            if (dropSelectItemSlot != item)
                            {
                                if (item != selectItemSlot)
                                {
                                    if (dropSelectItemSlot != null)
                                    {
                                        dropSelectItemSlot.ToggleBackgroundColor();
                                    }

                                    dropSelectItemSlot = item;

                                    dropSelectItemSlot.ToggleBackgroundColor();
                                    dropSelectItemSlot.UpdateGraphic();
                                }
                                else
                                {
                                    dropSelectItemSlot = null;
                                }
                            }

                            isOverlapWithItemSlot = true;

                            break;
                        }
                    }

                }

                if(!isOverlapWithItemSlot && dropSelectItemSlot != null)
                {
                    dropSelectItemSlot.ToggleBackgroundColor();

                    dropSelectItemSlot = null;
                }

            }
            else
            {            
                // find if is pointing to itemslot to start drag
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject != null)
                    {
                        ItemSlot item = result.gameObject.GetComponent<ItemSlot>();

                        if (item != null && item.isItemCanUse() && !isHelding && item.isFunctional)
                        {
                            isOverlapWithItemSlot = true;

                            StartCoroutine(DragHeldTimer(item));
                            break;
                        }
                    }

                }
            }
        }
        else
        {
            if(isDragging)
            {
                // drop
                if(dropSelectItemSlot != null)
                {
                    // check is drop item slot is not empty
                    if (dropSelectItemSlot.isItemCanUse())
                    {
                        dropSelectItemSlot.ToggleBackgroundColor();
                        selectItemSlot.ToggleBackgroundColor();

                        if (dropSelectItemSlot.item.itemIndex == selectItem.itemIndex)
                        {
                            // combine item
                            dropSelectItemSlot.AddSameItem(selectItemSlot.Count);
                        }
                        else
                        {
                            // switch item
                            selectItemSlot.item = selectItem;
                            dropSelectItemSlot.SwitchItem(ref selectItemSlot);
                        }

                    }
                    else
                    {
                        dropSelectItemSlot.item = selectItem;
                        dropSelectItemSlot.Count = selectItemSlot.Count;
                        dropSelectItemSlot.ToggleBackgroundColor();

                        selectItemSlot.Count = 0;
                        selectItemSlot.item = null;
                        selectItemSlot.UpdateGraphic();
                        selectItemSlot.ToggleBackgroundColor();

                    }

                    RemoveSelecting();
                }
                // replace item back
                else
                {
                    if (isSelectingItem())
                    {
                        selectItemSlot.item = selectItem;
                        selectItemSlot.UpdateGraphic();

                        selectItemSlot.ToggleBackgroundColor();
                        RemoveSelecting();
                    }
                    isDragging = false;
                    dropSelectItemSlot = null;
                    dragObject.SetActive(false);

                }
            }
            else
            {
                StopCoroutine("DragHeldTimer");
                isHelding = false;
            }

        }
        
    }


    IEnumerator DragHeldTimer(ItemSlot item)
    {
        if(isHelding)
        {
            yield return null;
        }

        isHelding = true;

        yield return new WaitForSeconds(0.3f);

        if(isClicking && item.item != null)
        {
            isHelding = false;

            Debug.Log("Drag");

            isDragging = true;

            if (isSelectingItem())
            {
                selectItemSlot.ToggleBackgroundColor();
                RemoveSelecting();
            }

            selectItem = item.item;
            selectItemSlot = item;
            selectItemSlot.ToggleBackgroundColor();
            selectItemSlot.item = null;
            selectItemSlot.UpdateGraphic();

            dragObject.SetActive(true);
            dragIcon.sprite = selectItem.icon;
            dragObject.transform.position = screenPosition;

        }
    }

    public void UpdateItemSlotsGraphics()
    {
        foreach(var itemSlot in itemSlotList.slotsList)
        {
            if(itemSlot.isItemCanUse())
                itemSlot.UpdateGraphic();
        }
    }
}
