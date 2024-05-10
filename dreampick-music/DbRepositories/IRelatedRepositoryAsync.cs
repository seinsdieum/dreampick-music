using System.Collections.Generic;
using System.Threading.Tasks;

namespace dreampick_music.DbRepositories;

public interface IRelatedRepositoryAsync<T2>
{
    Task<IEnumerable<T2>> GetRelated<T2>(string id);
    
    
    Task<int> GetRelatedCount<T2>(string id);
}