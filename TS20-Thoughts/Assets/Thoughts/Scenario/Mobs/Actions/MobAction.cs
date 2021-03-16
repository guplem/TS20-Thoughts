public abstract class MobAction
{
    public abstract void Execute(); 
    
    public override string ToString()
    {
        return this.GetType().Name.ToString();
    }

}
