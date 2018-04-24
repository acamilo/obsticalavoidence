
class SetTagRecursively extends ScriptableWizard
{
	var objToChange : GameObject;
	var tag : String;
	
	function OnWizardUpdate () {
		//objToChange = Selection.gameObject;
	}
	
	function OnWizardCreate () {
		//for(x in objToChange){
			var x = objToChange.GetComponentsInChildren(Transform);
			for(var z in x) z.tag = tag;
		//}
	}
	
	@MenuItem("SKiPPER/Set Tag")
	static function TagRecursively () {
		ScriptableWizard.DisplayWizard("Change Tags Recursively", SetTagRecursively, "Touch to Henshin...");
	}
}