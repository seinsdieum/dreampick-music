using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dreampick_music.DbRepositories;

public interface IRelationsRepositoryAsync<T1, T2>
{
    Task<IEnumerable<T2>> GetRelations(T1 entity);
    
    
    Task<int> GetRelationsCount(T1 entity);
}