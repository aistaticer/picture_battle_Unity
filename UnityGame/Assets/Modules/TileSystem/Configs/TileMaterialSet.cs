using UnityEngine;

[CreateAssetMenu(menuName = "Config/TileMaterialSet")] 
public class TileMaterialSet : ScriptableObject 
{ 
	public Material emptyMat; 
	public Material clickableTeamAMat; 
	public Material clickableTeamBMat; 
	public Material clickedTeamAMat; 
	public Material clickedTeamBMat; 
}	