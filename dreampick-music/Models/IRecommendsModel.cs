using System;
using System.Threading.Tasks;

namespace dreampick_music.Models;

public interface IRecommendsModel<T>
{
    Task<T> GetByFactors(params Func<bool>[] factors );

    Func<T, bool> BuildFactor(T item);
    
    
}