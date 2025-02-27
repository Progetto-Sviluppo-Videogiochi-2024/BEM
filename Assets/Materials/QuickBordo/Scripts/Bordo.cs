﻿//
//  Bordo.cs
//  QuickBordo
//
//  Created by Chris Nolet on 3/30/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

public class Bordo : MonoBehaviour
{
  private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();

  public enum Mode
  {
    BordoAll,
    BordoVisible,
    BordoHidden,
    BordoAndSilhouette,
    SilhouetteOnly
  }

  public Mode BordoMode
  {
    get { return bordoMode; }
    set
    {
      bordoMode = value;
      needsUpdate = true;
    }
  }

  public Color BordoColor
  {
    get { return bordoColor; }
    set
    {
      bordoColor = value;
      needsUpdate = true;
    }
  }

  public float BordoWidth
  {
    get { return bordoWidth; }
    set
    {
      bordoWidth = value;
      needsUpdate = true;
    }
  }

  [Serializable]
  private class ListVector3
  {
    public List<Vector3> data;
  }

  [SerializeField]
  private Mode bordoMode;

  [SerializeField]
  private Color bordoColor = Color.white;

  [SerializeField, Range(0f, 10f)]
  private float bordoWidth = 2f;

  [Header("Optional")]

  [SerializeField, Tooltip("Precompute enabled: Per-vertex calculations are performed in the editor and serialized with the object. "
  + "Precompute disabled: Per-vertex calculations are performed at runtime in Awake(). This may cause a pause for large meshes.")]
  private bool precomputeBordo;

  [SerializeField, HideInInspector]
  private List<Mesh> bakeKeys = new List<Mesh>();

  [SerializeField, HideInInspector]
  private List<ListVector3> bakeValues = new List<ListVector3>();

  private Renderer[] renderers;
  private Material bordoMaskMaterial;
  private Material bordoFillMaterial;

  private bool needsUpdate;

  void Awake()
  {

    // Cache renderers
    renderers = GetComponentsInChildren<Renderer>();

    // Instantiate bordo materials
    bordoMaskMaterial = Instantiate(Resources.Load<Material>(@"Materials/BordoMask"));
    bordoFillMaterial = Instantiate(Resources.Load<Material>(@"Materials/BordoFill"));

    bordoMaskMaterial.name = "BordoMask (Instance)";
    bordoFillMaterial.name = "BordoFill (Instance)";

    // Retrieve or generate smooth normals
    LoadSmoothNormals();

    // Apply material properties immediately
    needsUpdate = true;
  }

  void OnEnable()
  {
    foreach (var renderer in renderers)
    {

      // Append bordo shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Add(bordoMaskMaterial);
      materials.Add(bordoFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  void OnValidate()
  {

    // Update material properties
    needsUpdate = true;

    // Clear cache when baking is disabled or corrupted
    if (!precomputeBordo && bakeKeys.Count != 0 || bakeKeys.Count != bakeValues.Count)
    {
      bakeKeys.Clear();
      bakeValues.Clear();
    }

    // Generate smooth normals when baking is enabled
    if (precomputeBordo && bakeKeys.Count == 0)
    {
      Bake();
    }
  }

  void Update()
  {
    if (needsUpdate)
    {
      needsUpdate = false;

      UpdateMaterialProperties();
    }
  }

  void OnDisable()
  {
    foreach (var renderer in renderers)
    {

      // Remove bordo shaders
      var materials = renderer.sharedMaterials.ToList();

      materials.Remove(bordoMaskMaterial);
      materials.Remove(bordoFillMaterial);

      renderer.materials = materials.ToArray();
    }
  }

  void OnDestroy()
  {

    // Destroy material instances
    Destroy(bordoMaskMaterial);
    Destroy(bordoFillMaterial);
  }

  void Bake()
  {

    // Generate smooth normals for each mesh
    var bakedMeshes = new HashSet<Mesh>();

    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {

      // Skip duplicates
      if (!bakedMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }

      // Serialize smooth normals
      var smoothNormals = SmoothNormals(meshFilter.sharedMesh);

      bakeKeys.Add(meshFilter.sharedMesh);
      bakeValues.Add(new ListVector3() { data = smoothNormals });
    }
  }

  void LoadSmoothNormals()
  {

    // Retrieve or generate smooth normals
    foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    {

      // Skip if smooth normals have already been adopted
      if (!registeredMeshes.Add(meshFilter.sharedMesh))
      {
        continue;
      }

      // Retrieve or generate smooth normals
      var index = bakeKeys.IndexOf(meshFilter.sharedMesh);
      var smoothNormals = (index >= 0) ? bakeValues[index].data : SmoothNormals(meshFilter.sharedMesh);

      // Store smooth normals in UV3
      meshFilter.sharedMesh.SetUVs(3, smoothNormals);

      // Combine submeshes
      var renderer = meshFilter.GetComponent<Renderer>();

      if (renderer != null)
      {
        CombineSubmeshes(meshFilter.sharedMesh, renderer.sharedMaterials);
      }
    }

    // Clear UV3 on skinned mesh renderers
    foreach (var skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
    {

      // Skip if UV3 has already been reset
      if (!registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
      {
        continue;
      }

      // Clear UV3
      skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];

      // Combine submeshes
      CombineSubmeshes(skinnedMeshRenderer.sharedMesh, skinnedMeshRenderer.sharedMaterials);
    }
  }

  List<Vector3> SmoothNormals(Mesh mesh)
  {

    // Group vertices by location
    var groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

    // Copy normals to a new list
    var smoothNormals = new List<Vector3>(mesh.normals);

    // Average normals for grouped vertices
    foreach (var group in groups)
    {

      // Skip single vertices
      if (group.Count() == 1)
      {
        continue;
      }

      // Calculate the average normal
      var smoothNormal = Vector3.zero;

      foreach (var pair in group)
      {
        smoothNormal += smoothNormals[pair.Value];
      }

      smoothNormal.Normalize();

      // Assign smooth normal to each vertex
      foreach (var pair in group)
      {
        smoothNormals[pair.Value] = smoothNormal;
      }
    }

    return smoothNormals;
  }

  void CombineSubmeshes(Mesh mesh, Material[] materials)
  {

    // Skip meshes with a single submesh
    if (mesh.subMeshCount == 1)
    {
      return;
    }

    // Skip if submesh count exceeds material count
    if (mesh.subMeshCount > materials.Length)
    {
      return;
    }

    // Append combined submesh
    mesh.subMeshCount++;
    mesh.SetTriangles(mesh.triangles, mesh.subMeshCount - 1);
  }

  void UpdateMaterialProperties()
  {

    // Apply properties according to mode
    bordoFillMaterial.SetColor("_BordoColor", bordoColor);

    switch (bordoMode)
    {
      case Mode.BordoAll:
        bordoMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        bordoFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        bordoFillMaterial.SetFloat("_BordoWidth", bordoWidth);
        break;

      case Mode.BordoVisible:
        bordoMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        bordoFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        bordoFillMaterial.SetFloat("_BordoWidth", bordoWidth);
        break;

      case Mode.BordoHidden:
        bordoMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        bordoFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        bordoFillMaterial.SetFloat("_BordoWidth", bordoWidth);
        break;

      case Mode.BordoAndSilhouette:
        bordoMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        bordoFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
        bordoFillMaterial.SetFloat("_BordoWidth", bordoWidth);
        break;

      case Mode.SilhouetteOnly:
        bordoMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
        bordoFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
        bordoFillMaterial.SetFloat("_BordoWidth", 0f);
        break;
    }
  }
}
