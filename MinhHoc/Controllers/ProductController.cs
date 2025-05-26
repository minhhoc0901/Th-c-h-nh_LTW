using Microsoft.AspNetCore.Mvc;
using MinhHoc.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using MinhHoc.Repository;
namespace MinhHoc.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        public IActionResult Add()
        {
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }
        //[HttpPost]
        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl, List<IFormFile> imageUrls)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện 
                    product.ImageUrl = await SaveImage(imageUrl);
                }

                if (imageUrls != null)
                {
                    product.ImageUrls = new List<string>();
                    foreach (var file in imageUrls)
                    {
                        // Lưu các hình ảnh khác 
                        product.ImageUrls.Add(await SaveImage(file));
                    }
                }

                _productRepository.Add(product);
                return RedirectToAction("Index");
            }

            return View(product);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            // Thay đổi đường dẫn theo cấu hình của bạn 
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName; // Trả về đường dẫn tương đối 
        }
        public IActionResult Index()
        {
            var products = _productRepository.GetAll();
            return View(products);
        }
        public IActionResult Display(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Show the product update form 
        public IActionResult Update(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            // Thêm categories vào ViewBag
            var categories = _categoryRepository.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            return View(product);
        }
        // Process the product update 
        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                // Thêm categories vào ViewBag
                var categories = _categoryRepository.GetAllCategories();
                ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

                return View(product);
            }

            var existingProduct = _productRepository.GetById(product.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // Cập nhật các thông tin khác
            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Description = product.Description;
            existingProduct.CategoryId = product.CategoryId;

            if (imageFile != null && imageFile.Length > 0)
            {
                // Lưu file ảnh mới vào wwwroot/images (thư mục bạn muốn)
                var fileName = Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh mới
                existingProduct.ImageUrl = "/images/" + fileName;
            }
            // Nếu không có file mới thì giữ nguyên ảnh cũ

            _productRepository.Update(existingProduct);

            return RedirectToAction("Index");
        }
        // Show the product delete confirmation 
        public IActionResult Delete(int id)
        {
            var product = _productRepository.GetById(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            _productRepository.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
