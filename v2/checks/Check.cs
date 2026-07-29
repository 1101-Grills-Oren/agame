namespace agame.checks;
class Check{
	public string name;
	public int category;
  public bool obtained=false;
  public Action? onObtained=null;
  public Check(string name, int category){
    this.name=name;
    this.category=category;
  }
  public Check(string name, int category, Action onObtained){
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
