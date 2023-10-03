using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class MapObject : MonoBehaviour
{

    [Header("Resource Settings")]
    [SerializeField] private int gatherAmountAllowed = 3;
    [SerializeField] private ResourceType _resourceType;
    [SerializeField] private ItemListAmount[] resource;
    public MeshRenderer meshRenderer;


    private bool isDissolving = false;
    [Space]
    
    [Header("Dissolve Animation Properties")]
    public float dissolveRate = 0.05f;
    public float refreshRate = 0.025f;
    
    
    
    private Material[] skinnedMaterial;
    private int currentIndex = 0;
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    

    private void Start()
    {


        
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh mesh = Instantiate(meshFilter.sharedMesh);
        mesh.MarkDynamic();
        meshFilter.mesh = mesh;
        
        
        if (meshRenderer != null)
            skinnedMaterial = meshRenderer.materials;
        else
        {
            meshRenderer = GetComponent<MeshRenderer>();
            skinnedMaterial = meshRenderer.materials;
        }


    }
    

    public void UpdateStage()
    {
        currentIndex++;

        if (currentIndex >= gatherAmountAllowed && !isDissolving)
        {
            isDissolving = true;
            StartCoroutine(Dissolve());
        }
        
    }
    
    public enum ObjectStage
    {
        PRISTINE = 0,
        DAMAGED = 1,
        BROKEN = 2
    }
    
    
    public void GatherObject(PlayerController controller)
    {
        Debug.Log("Gathering : " + resource[0].resource + "  Amount : " + resource[0].amount);
        //Sends information about this object to the quest mnager to check if it satisfies conditions.
        //TODO: Implement inventory
        //controller.QuestManager.IncreaseAmountCount(1, _resourceType);
        
        //Update stage of the object.
        UpdateStage();

        //Collect item into the inventory.
        foreach (var resource in resource)
        {
            FindObjectOfType<InventoryManagerNew>().AddItemToInventory(resource.resource, resource.amount);
            
        }
        
       
    }

    //Called when object stage reaches past index set, uses shader graph to affect alpha of material.
    public IEnumerator Dissolve()
    {
        //Debug.Log("Dissolving: " + skinnedMaterial[0].GetFloat(DissolveAmount));
        if (skinnedMaterial.Length > 0)
        {
            float counter = 0;
            while (skinnedMaterial[0].GetFloat(DissolveAmount) < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < skinnedMaterial.Length; i++)
                {
                    skinnedMaterial[i].SetFloat(DissolveAmount, counter);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
        Destroy(gameObject);
    }
    
}


