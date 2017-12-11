using UnityEngine;
using System.Collections;

public class SkillIcon : MonoBehaviour 
{
	public Texture[] skillIcon;
	private Texture mySkillIcon;
	public Texture skillCooldown;
	public Texture skillActivated;

	private Rect skillIconRect;
	private Rect skillCooldownRect;
	private Rect skillActivatedRect;

	private Rect screenRect;
	private Rect INTENDED_RES = new Rect(0, 0, 1280, 1024);

	private BaseSkillHandler mySkillHandler = null;

	void Awake()
	{
		this.enabled = false;
	}

	// Use this for initialization
	void Start() 
	{
		screenRect = AspectUtility.screenRect;

		skillIconRect = new Rect(screenRect.x + (screenRect.width / INTENDED_RES.width * 10.0f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 89.0f),
									screenRect.width / INTENDED_RES.width * 222.0f,
									screenRect.height / INTENDED_RES.height * 79.0f);

		skillCooldownRect = new Rect(screenRect.x + (screenRect.width / INTENDED_RES.width * 15.0f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 84.0f),
									screenRect.width / INTENDED_RES.width * 212.0f,
									screenRect.height / INTENDED_RES.height * 69.0f);

		skillActivatedRect = new Rect(screenRect.x + (screenRect.width / INTENDED_RES.width * 6.0f),
									screenRect.y + screenRect.height - (screenRect.height / INTENDED_RES.height * 93.0f),
									screenRect.width / INTENDED_RES.width * 230.0f,
									screenRect.height / INTENDED_RES.height * 87.0f);

		mySkillHandler = PlayerData.characters[PlayerData.color].GetComponent<BaseSkillHandler>();
		mySkillIcon = skillIcon[PlayerData.classId[PlayerData.color]];
	}
	
	// Update is called once per frame
	void Update() 
	{
	}

	void OnGUI()
	{
		if (Application.loadedLevel > (int)LevelManager.Scene.Level_1)
		{
			GUI.DrawTexture(skillIconRect, mySkillIcon);

			if (mySkillHandler.powerUpIsEnabled)
			{
				GUI.DrawTexture(skillCooldownRect, skillCooldown);
				GUI.DrawTexture(skillActivatedRect, skillActivated);
			}
			
			if (mySkillHandler.powerUpOnCooldown)
			{
				GUI.DrawTexture(ClipRectWidth(skillCooldownRect, mySkillHandler.CooldownValue), 
								skillCooldown, 
								ScaleMode.ScaleAndCrop);
			}
		}
	}

	private Rect ClipRectWidth(Rect rect, float widthValue)
	{
		if (widthValue != 1.0f)
			rect.width *= widthValue;

		return rect;
	}
}
