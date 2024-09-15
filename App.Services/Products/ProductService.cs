using App.Repositories;
using App.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace App.Services.Products;

public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<ServiceResult<List<ProductDto>>> GetTopPriceProductsAsync(int count)
    {
        List<Product> products = await productRepository.GetTopPriceProductsAsync(count);

        List<ProductDto> productsAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productsAsDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetPagedAllListAsync(int pageNumber, int pageSize)
    {
        List<Product> products = await productRepository.GetAll().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        List<ProductDto> productAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productAsDto);
    }

    public async Task<ServiceResult<List<ProductDto>>> GetAllListAsync()
    {
        List<Product> products = await productRepository.GetAll().ToListAsync();

        List<ProductDto> productAsDto = products.Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock)).ToList();

        return ServiceResult<List<ProductDto>>.Success(productAsDto);
    }

    public async Task<ServiceResult<ProductDto?>> GetByIdAsync(int id)
    {
        Product? product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            ServiceResult<ProductDto>.Fail("Product not found!", HttpStatusCode.NotFound);
        }

        ProductDto productAsDto = new ProductDto(product!.Id, product.Name, product.Price, product.Stock);

        return ServiceResult<ProductDto>.Success(productAsDto)!;
    }

    public async Task<ServiceResult<CreateProductResponse>> CreateAsync(CreateProductRequest request)
    {
        Product product = new()
        {
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
        };

        await productRepository.AddAsync(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult<CreateProductResponse>.SuccessAsCreated(new CreateProductResponse(product.Id), $"api/products({product.Id})");
    }

    public async Task<ServiceResult> UpdateAsync(int id, UpdateProductRequest request)
    {
        Product? product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return ServiceResult.Fail("Product not found!", HttpStatusCode.NotFound);
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;

        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> UpdateStockAsync(UpdateProductStockRequest request)
    {
        Product? product = await productRepository.GetByIdAsync(request.ProductId);

        if (product is null)
        {
            return ServiceResult.Fail("Product not found!", HttpStatusCode.NotFound);
        }

        product.Stock = request.Quantity;
        productRepository.Update(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }

    public async Task<ServiceResult> DeleteAsync(int id)
    {
        Product? product = await productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return ServiceResult.Fail("Product not found!", HttpStatusCode.NotFound);
        }

        productRepository.Delete(product);
        await unitOfWork.SaveChangesAsync();

        return ServiceResult.Success(HttpStatusCode.NoContent);
    }
}