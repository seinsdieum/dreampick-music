using System.Collections.Generic;
using System.Threading.Tasks;

namespace dreampick_music.DbRepositories;


public interface IRepositoryAsync<T>
{
    Task<T> GetById(string id); // Получить объект по идентификатору
    Task<IEnumerable<T>> GetAll(); // Получить все объекты
    void Add(T entity); // Добавить новый объект
    Task Update(T entity); // Обновить объект
    Task Delete(T entity); // Удалить объект
}