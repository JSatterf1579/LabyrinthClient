using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    private int attackModifier;
    private int attackRange;
    private AttackPattern pattern;
    private string image;
    private string name;
    private string description;

    private int[,] damageMap;


    public void Init(string name, string image, int attackModifier, int range, AttackPattern pattern, string description)
    {
        this.name = name;
        this.image = image;
        this.description = description;
        this.attackModifier = attackModifier;
        attackRange = range;
        this.pattern = pattern;
    }

    // Use this for initialization
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
