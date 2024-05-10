using System.Collections.Generic;
using System.Threading.Tasks;

namespace dreampick_music.DbRepositories;

public interface IRelatedRepositoryAsync<T1, T2>
{
    Task<IEnumerable<T2>> GetRelated(T1 entity);
    
    
    Task<int> GetRelatedCount(T1 entity);
}