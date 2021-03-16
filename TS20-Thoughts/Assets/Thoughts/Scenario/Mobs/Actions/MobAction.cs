public abstract class MobAction
{
    public string objectiveName;
    
    public MobAction(string ObjectiveName)
    {
        this.objectiveName = ObjectiveName;
    }

    public override string ToString()
    {
        return $"MobAction: {objectiveName}";
    }

}
