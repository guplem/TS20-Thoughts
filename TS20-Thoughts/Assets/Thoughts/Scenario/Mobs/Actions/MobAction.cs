public class MobAction
{
    public string objectiveName;
    
    public MobAction(string gameObjectName)
    {
        this.objectiveName = gameObjectName;
    }

    public override string ToString()
    {
        return $"MobAction: {objectiveName}";
    }

}
