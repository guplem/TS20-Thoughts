using Thoughts.Mobs;

public abstract class MobAction
{
    public abstract void Execute(Mob mob); 
    
    public override string ToString()
    {
        return this.GetType().Name;
    }

}
