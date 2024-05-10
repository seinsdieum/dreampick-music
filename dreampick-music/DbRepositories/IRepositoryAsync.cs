using System.Collections.Generic;
using System.Threading.Tasks;

namespace dreampick_music.DbRepositories;


public interface IRepositoryAsync<T>
{
    Task<T> GetById(int id); // Получить объект по идентификатору
    IEnumerable<T> GetAll(); // Получить все объекты
    Task Add(T entity); // Добавить новый объект
    Task Update(T entity); // Обновить объект
    Task Delete(T entity); // Удалить объект
}