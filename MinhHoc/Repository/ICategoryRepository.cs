using MinhHoc.Models;

namespace MinhHoc.Repository
{
    public interface ICategoryRepository
    {
        IEnumerable<Category> GetAllCategories();
    }
}
