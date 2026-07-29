namespace agame.checks;
class Check{
	public string name;
	public int category;
	public string id;
	public bool obtained=false;
	public Action? onObtained=null;
	public Check(string name,string id, int category){
		this.name=name;
		this.id=id;
		this.category=category;
	}
	public Check(string name, string id, int category, Action onObtained){
		this.id=id;
		this.name=name;
		this.category=category;
		this.onObtained=onObtained;
	}
	public void obtain(){
		if(obtained==false){
			obtained=true;
			if(onObtained!=null)
				onObtained();
		}
	}
}
