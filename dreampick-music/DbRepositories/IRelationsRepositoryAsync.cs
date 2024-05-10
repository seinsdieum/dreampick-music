using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dreampick_music.DbRepositories;

public interface IRelationsRepositoryAsync<T2>
{
    Task<IEnumerable<T2>> GetRelations(string id);
    
    
    Task<int> GetRelationsCount(string id);
}