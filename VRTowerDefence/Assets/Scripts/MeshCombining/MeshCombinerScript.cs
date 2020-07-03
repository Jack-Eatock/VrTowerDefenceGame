using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseMaterialsAgainstDependencies {

    public Material BaseMaterial;

    public List<MeshFilter> MeshFiltersThatRequireBaseMaterial = new List<MeshFilter>();
    public List<int> MeshFilterSubIndex = new List<int>();


    public Mesh SubMesh = null;
}


public class MeshCombinerScript : MonoBehaviour
{
    public GameObject MeshHolder;
    public GameObject MeshStorage;

  //  public MeshFilter myMeshFilter;
    public int maxNumberOfVerticesForSubMeshes;
    public int maxNumberOfVerticesForMeshes;

    private List<GameObject> Meshes = new List<GameObject>();

    public List<BaseMaterialsAgainstDependencies> BaseMaterialsRequired = new List<BaseMaterialsAgainstDependencies>();

    public void MyOwnAdvancedMeshCombinder()
    {
        BaseMaterialsRequired.Clear();

        // First decide what to actually merge.

        List<GameObject> objectsToMerge = new List<GameObject>();
        
        // Only Add filters we want to affect.

        foreach (MeshFilter filter in transform.GetComponentsInChildren<MeshFilter>(false)) // Loops through every MeshFilter that is active on its children.
        {
            if (filter.transform.tag != "IgnoreFromMeshMerge" && filter.transform != transform)
            {
                objectsToMerge.Add(filter.gameObject);
            }

        }

        // Now that we know what to merge.

        //Debug.Log("Number of objects ready to merge: " + objectsToMerge.Count);

        // Now split the objects mesh filters into lists based on their Material.
        int localMatSubIndex = 0;

        foreach (GameObject obj in objectsToMerge)
        {
            // Get all the materials on this object.
            Material[] localMaterials = obj.GetComponent<Renderer>().sharedMaterials; // Usually only one material. 

            localMatSubIndex = 0;

            // Loop through all the local materials, to check if you need to add another material.
            foreach (Material localMat in localMaterials)
            {
                bool isTheMaterialAlreadySetUP = false;

                // If the locally required material is already defined. Then don't worry about it. Otherwise add it.
                foreach (BaseMaterialsAgainstDependencies baseMat in BaseMaterialsRequired)
                {
                    //Debug.Log("baseMat " + localMat);

                    if (baseMat.BaseMaterial == localMat)
                    {
                        // This material already has a dependency set up, soo add its mesh filter. 
                        // But first check their wont be too many vertices.
                        MeshFilter localMaterialsFilter = obj.GetComponent<MeshFilter>();

                        if (VerifyNumOfVertices(baseMat, localMaterialsFilter)) // If there is enough vertices spare to add the mesh.   // If it is not verified then The materialAlreadySetUp will stay false.
                        {                                                                                                               // Causing another sub mesh to be generated.
                            baseMat.MeshFiltersThatRequireBaseMaterial.Add(localMaterialsFilter);
                            baseMat.MeshFilterSubIndex.Add(localMatSubIndex);
                            isTheMaterialAlreadySetUP = true;
                            break;
                        }
                    }
                    

                }


                if (!isTheMaterialAlreadySetUP) // The material has not been set up as a dependancy
                {
                    // Create a new BaseMaterial
                    // Set its base material
                    BaseMaterialsAgainstDependencies newBaseMat = new BaseMaterialsAgainstDependencies();
                    newBaseMat.BaseMaterial = localMat;

                    // Add the material that relies on it's mesh filter.
                    MeshFilter localMaterialsFilter = obj.GetComponent<MeshFilter>();
                    newBaseMat.MeshFiltersThatRequireBaseMaterial.Add(localMaterialsFilter);
                    newBaseMat.MeshFilterSubIndex.Add(localMatSubIndex);

                    // Finally add it as a new base material.  
                    BaseMaterialsRequired.Add(newBaseMat);

                }

                localMatSubIndex++;
            }
        }

        // Now that we have a 2d array of the Mesh filters against their required Material. We should Squish them down into a submesh per Material.
        int index;

        foreach (BaseMaterialsAgainstDependencies baseMat in BaseMaterialsRequired)
        {
            //Debug.Log("Material " + baseMat.BaseMaterial + " Has " + baseMat.MeshFiltersThatRequireBaseMaterial.Count + "Dependencies." );

            // Create a list of CombineInstances to store all of the info we need for each filter within this current material to combine into one. 

            List<CombineInstance> combinersForEachFilterPerMaterial = new List<CombineInstance>();


            index = 0;

            foreach (MeshFilter filter in baseMat.MeshFiltersThatRequireBaseMaterial)
            {
                CombineInstance Ci = new CombineInstance();

                if (!filter.sharedMesh.isReadable)
                {
                    Debug.Log(filter.transform.parent.transform.name + " - Is not readable!!!");
                }
              

                Ci.mesh = filter.sharedMesh;
                Ci.subMeshIndex = baseMat.MeshFilterSubIndex[index];
                Ci.transform = filter.transform.localToWorldMatrix;

                combinersForEachFilterPerMaterial.Add(Ci);

                index++;
            }

            // Creates a new mesh that is a combination of all the meshes that require this material.

            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(combinersForEachFilterPerMaterial.ToArray(), true);
            baseMat.SubMesh = newMesh;

            //Debug.Log( "This Submesh has: " + baseMat.SubMesh.vertexCount + " Vertices");

        }


        // Finally combine all the meshes.
        
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        List<Material> currentMaterialsRequired = new List<Material>();
        int currentVertices = 0;

        //Debug.Log(" LOOK AT :" + BaseMaterialsRequired.Count);

        for (int baseMaterialIndex = 0; baseMaterialIndex < BaseMaterialsRequired.Count; baseMaterialIndex++)
        {
           
            CombineInstance ci = new CombineInstance();
            BaseMaterialsAgainstDependencies baseMat = BaseMaterialsRequired[baseMaterialIndex];
  
            ci.mesh = baseMat.SubMesh;
            ci.transform = transform.worldToLocalMatrix;
            ci.subMeshIndex = 0;
            
           // Debug.Log("Adding " + baseMat.BaseMaterial + " with " + ci.mesh.vertexCount);

            int verticesInMesh = ci.mesh.vertexCount;

            if (currentVertices + verticesInMesh > maxNumberOfVerticesForMeshes) // Need to create a new mesh, Over the Mesh Vertex limit.
            {
           
                // Create Mesh for last. 
                CreateNewMesh(finalCombiners.ToArray(), currentMaterialsRequired.ToArray());

                // Clear everything.
                currentMaterialsRequired.Clear();
                currentVertices = 0;
                finalCombiners.Clear();

                // Set up for next mesh.

                finalCombiners.Add(ci);
                currentMaterialsRequired.Add(baseMat.BaseMaterial);
                currentVertices += verticesInMesh;

                // On the odd Chance that this is also the last run. 
                if (baseMaterialIndex == BaseMaterialsRequired.Count - 1)
                {
                    // Create new mesh.
                    CreateNewMesh(finalCombiners.ToArray(), currentMaterialsRequired.ToArray());

                    // Clear for next use.
                    currentMaterialsRequired.Clear();
                    currentVertices = 0;
                    finalCombiners.Clear();

                    Debug.Log("Fnished Generating Mesh");

                    GameModeSurvivalScript.GenerationTicker = 4;
                }

            }

            else if ( baseMaterialIndex == BaseMaterialsRequired.Count - 1) // This is the final run. So finish up and create the new mesh.
            {
                // Make up for final run
                finalCombiners.Add(ci);
                currentMaterialsRequired.Add(baseMat.BaseMaterial);



                // Create new mesh.
                CreateNewMesh(finalCombiners.ToArray(), currentMaterialsRequired.ToArray());

                // Clear for next use.
                currentMaterialsRequired.Clear();
                currentVertices = 0;
                finalCombiners.Clear();

                Debug.Log("Fnished Generating Mesh");

                GameModeSurvivalScript.GenerationTicker = 4;
            }

            else // Keep Looping.
            {
                currentVertices += verticesInMesh;
                finalCombiners.Add(ci);
                currentMaterialsRequired.Add(baseMat.BaseMaterial);

            }

        }
      

        SetActiveAllChildren(transform, false);
    }

  
    private void CreateNewMesh(CombineInstance[] finalCombiners, Material[] currentMaterialsRequired)
    {
        GameObject newMesh = GameObject.Instantiate(MeshHolder, MeshStorage.transform);

        Meshes.Add(newMesh);

        Mesh meshy = new Mesh();
        meshy.CombineMeshes(finalCombiners, false);

        newMesh.GetComponent<MeshFilter>().sharedMesh = meshy;
        newMesh.GetComponent<MeshRenderer>().sharedMaterials = currentMaterialsRequired;
    }

    public bool VerifyNumOfVertices(BaseMaterialsAgainstDependencies BaseMat, MeshFilter meshFilterToAdd) // Checks if the subMesh will have too many Vertices.
    {

        float currentNumOfVertices = 0;

        foreach (MeshFilter localFilter in BaseMat.MeshFiltersThatRequireBaseMaterial)
        {
            //Debug.Log(localFilter.transform.name);
            currentNumOfVertices += localFilter.sharedMesh.vertexCount;
        }

        int numOfVerticesToAdd = meshFilterToAdd.sharedMesh.vertexCount;

        // Ensures that there are not too many vertices.

        if (currentNumOfVertices + numOfVerticesToAdd > maxNumberOfVerticesForSubMeshes) // If there are too many Split it .
        {
            return false;
        }

        return true;

    }

    private void SetActiveAllChildren(Transform transform, bool value)
    {
        foreach (Transform child in transform)
        {
            if (child.transform.tag != "IgnoreFromMeshMerge")
            {
                child.gameObject.SetActive(value);

                SetActiveAllChildren(child, value);
            }
        }
    }




    public void DestroyMesh()
    {
        SetActiveAllChildren(transform, true);

        foreach (GameObject mesh in Meshes)
        {
            Destroy(mesh);
        }

        Meshes.Clear();

        Debug.Log("Destroyed Mesh");
    }








































    /*


    public void AdvancedMerge()
    {

        Vector3 oldPos = transform.position;
        Quaternion oldRotation = transform.rotation;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        // All our children (and us)
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(false);

        // All the meshes in our children (just a big list)
        List<Material> materials = new List<Material>();

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false); // <-- you can optimize this
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform || renderer.transform.tag == "IgnoreFromMerge")
                continue;


            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                    materials.Add(localMat);
        }

        // Each material will have a mesh for it.
        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials)
        {
            // Make a combiner for each (sub)mesh that is mapped to the right material.
            List <CombineInstance> combiners = new List<CombineInstance>();
            foreach (MeshFilter filter in filters)
            {
                if (filter.transform == transform || filter.transform.tag == "IgnoreFromMerge") continue;
                // The filter doesn't know what materials are involved, get the renderer.
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();  // <-- (Easy optimization is possible here, give it a try!)
                if (renderer == null)
                {
                    Debug.LogError(filter.name + " has no MeshRenderer");
                    continue;
                }

                // Let's see if their materials are the one we want right now.
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)
                        continue;
                    // This submesh is the material we're looking for right now.
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex;
                    ci.transform = filter.transform.localToWorldMatrix;
                    combiners.Add(ci);
                }
            }
            // Flatten into a single mesh.
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(), true);
            submeshes.Add(mesh);
        }

        // The final mesh: combine all the material-specific meshes as independent submeshes.
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            finalCombiners.Add(ci);
        }
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
        myMeshFilter.sharedMesh = finalMesh;
        Debug.Log("Final mesh has " + submeshes.Count + " materials.");

        transform.rotation = oldRotation;
        transform.position = oldPos;


        SetActiveAllChildren(transform, false);
    }




  




    void AttemptToCombineMesh()
    {
        Debug.Log("Combine Mesh");

        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>(false);

        Debug.Log("Filters " + filters.Length);

        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(false);

        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform) // Don't include yourself!!! selfish!
            {
                continue;
            }

            Material[] localMats = renderer.sharedMaterials;

            foreach (Material localMat in localMats) // Ensures that you only add a material once. No dupes.
            {
                if (!materials.Contains(localMat))
                {
                    materials.Add(localMat);
                }
            }

        }


        List<Mesh> submeshes = new List<Mesh>();

        foreach (Material material in materials)
        {
            // make a combiner for each (sub) mesh, that is mapped to a specific material.
            List<CombineInstance> combiners = new List<CombineInstance>();

            foreach (MeshFilter filter in filters)
            {
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();

                if (renderer == null)
                {
                    Debug.LogError(filter.name + " has no MeshRenderer");
                    continue;
                }

                // see if their materials are the one we want.

                Material[] localMaterials = renderer.sharedMaterials; // every material attached to this mesh filter.

                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)
                    {
                        continue;
                    }

                    // The local material being checked is the same as the main material. Sooo Add it as being  dependant.

                    CombineInstance ci = new CombineInstance();
                    ci.mesh = filter.sharedMesh; // Adds this current mesh as a "Mesh that uses this specific material"
                    ci.subMeshIndex = materialIndex;
                    ci.transform = Matrix4x4.identity;
                    combiners.Add(ci);


                }





            }


            // flatten all the meshes that use the specific material into one singular mesh.
            Mesh mesh = new Mesh();

            Debug.Log(" UW " + mesh.isReadable);

            mesh.CombineMeshes(combiners.ToArray(), true);
            submeshes.Add(mesh);

            Debug.Log("Mesh of " + combiners.Count);

        }

        // The final mesh.

        List<CombineInstance> finalCombiners = new List<CombineInstance>();

        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            finalCombiners.Add(ci);
        }

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);



        MeshFilter meshFilter = GetComponent<MeshFilter>();
        myMeshFilter.sharedMesh = finalMesh;

        Debug.Log("Final mesh has " + submeshes.Count + " materials");



    }

    */
}
