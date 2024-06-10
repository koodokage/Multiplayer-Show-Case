using System.Collections.Generic;

public abstract class AEntityFactory<T,K> : ASingleBehaviour<AEntityFactory<T,K>>
{ 
    protected string prefabResourcePath;
    protected List<T> pool_Entity;
    public abstract T Produce(K controller);
    public abstract T[] Produce(K controller,int amount);
    public abstract void Release(T entity);

}
