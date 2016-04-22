using System.Collections;

public class TempCard {

	public bool IsMonster { private set; get; }

	public string Name { private set; get; }

	public int cost { private set; get; }

	public int count { get; set; }

	public TempCard(string name, int cost, bool isMonster, int quantity) {
		Name = name;
		this.cost = cost;
		IsMonster = isMonster;
		this.count = quantity;
	}

}
