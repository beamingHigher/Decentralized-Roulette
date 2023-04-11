using UnityEngine;
using System.Collections;

namespace BLabRouletteProject {

public class BBChipData : MonoBehaviour {
  
   public int posID;
   public float betValue;
   public string s_posID;	
   public bool canRemoveChip = true;
   public bool wonChip = false;
		
   public Material greenMat;
   public Material redMat;
   public Material blueMat;
   public Material doubleMat;
   
	
  public MeshRenderer meshRend;	

  public GameObject resultCanvas; 
}
}