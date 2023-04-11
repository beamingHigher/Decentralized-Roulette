using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BLabRouletteProject {

public class BBMoveObjectsController : MonoBehaviour {

    public Transform playerChipsPoint;
	public Transform casinoChipsPoint;


	[HideInInspector]
	public bool moveMultiObjectMoveList = false;

	public List <MultiObjectMove> multiObjectMoveList = new List<MultiObjectMove>();

	public struct MultiObjectMove {
	  public Transform toMove;
	  public Transform destination;
	};

	public GameObject touchBettingPosRoot;

	public string winnerNumber = "";

	private  MeshRenderer[] allBetPos;

	[HideInInspector]
	public bool gotHandEnds = false;

	public AudioClip clipCleanTable;

	// Use this for initialization
	void Start () {
		allBetPos = touchBettingPosRoot.GetComponentsInChildren<MeshRenderer>();
	}

	void FixedUpdate () {

		if(moveMultiObjectMoveList) {
				float step = 5 * Time.deltaTime;
			   int notStillMove = multiObjectMoveList.Count;
			    for(int x = 0; x < multiObjectMoveList.Count;x++) {
			 	 multiObjectMoveList[x].toMove.position = Vector3.MoveTowards(multiObjectMoveList[x].toMove.position,  multiObjectMoveList[x].destination.position, step);
				 float dist = Vector3.Distance(multiObjectMoveList[x].toMove.position, multiObjectMoveList[x].destination.position);
				    if(dist < 0.01f) { 
						notStillMove--;
					 }
				}
			if(notStillMove < 1) {
				moveMultiObjectMoveList = false;
				multiObjectMoveList.Clear();
			}
		}

	}


	IEnumerator _startMoveChips() {
		gotHandEnds = true;

	   gameObject.GetComponent<AudioSource>().PlayOneShot(clipCleanTable);
	   
	    List<GameObject> wonPosList = new List<GameObject>();

	    foreach(MeshRenderer mr in allBetPos) {
	       if(mr.gameObject.name.Contains("_")) {
	         string[] splitted = mr.gameObject.name.Split('_');
	           foreach(string s in splitted) {
	                if(s == winnerNumber) {
	                  wonPosList.Add(mr.gameObject);
	                  mr.enabled = true;
	                }
	           }
	       } else {
              if(mr.gameObject.name == winnerNumber) {
                wonPosList.Add(mr.gameObject);
                mr.enabled = true;
              }
	       }
	    }

//	    foreach(GameObject g in wonPosList) Debug.Log("wonPos : " + g.name);

		multiObjectMoveList.Clear();

		GameObject[] allChips = GameObject.FindGameObjectsWithTag("betChip");

		List<GameObject> wonChips = new List<GameObject>();
		List<GameObject> loseChips = new List<GameObject>();


		foreach(GameObject g in allChips) {
		   if(!g.GetComponent<BBChipData>().wonChip) {
				//g.tag = "loseChip";
              loseChips.Add(g);
		   } else {
              wonChips.Add(g);
		   }
		}

		yield return new WaitForSeconds(1);

		//GameObject[] wonChips = GameObject.FindGameObjectsWithTag("betChip");
		//GameObject[] loseChips = GameObject.FindGameObjectsWithTag("loseChip");

		foreach(GameObject g in wonChips) {
			MultiObjectMove mc = new MultiObjectMove();
		    mc.toMove = g.transform;
		    Transform tmpT = playerChipsPoint;
		    tmpT.position = new Vector3(tmpT.position.x + UnityEngine.Random.Range(0.001f,0.005f),tmpT.position.y,tmpT.position.z);
		    mc.destination = tmpT;//playersChipEndingPoint[currentActivePlayer];
		    multiObjectMoveList.Add(mc);
			g.tag = "movedChip";
		}

		foreach(GameObject g in loseChips) {
			MultiObjectMove mc = new MultiObjectMove();
		    mc.toMove = g.transform;
		    Transform tmpT = casinoChipsPoint;
		    tmpT.position = new Vector3(tmpT.position.x + UnityEngine.Random.Range(0.001f,0.005f),tmpT.position.y,tmpT.position.z);
		    mc.destination = tmpT;//playersChipEndingPoint[currentActivePlayer];
		    multiObjectMoveList.Add(mc);
			g.tag = "movedChip";
		}

		moveMultiObjectMoveList = true;
	}

	public void startMoveChips() {
	  StartCoroutine( _startMoveChips() );
	}

	public void resetAllBettingPos() {
	    gotHandEnds = false;
		foreach(MeshRenderer mr in allBetPos) {
		  mr.enabled = false;
		}

		StartCoroutine( cleanMovedChips() );
	}

	IEnumerator cleanMovedChips() {
	   yield return new WaitForSeconds(10);
		GameObject[] movedChips = GameObject.FindGameObjectsWithTag("movedChip");

		foreach(GameObject g in movedChips) {
		    Destroy(g);
		}

	}

}
}