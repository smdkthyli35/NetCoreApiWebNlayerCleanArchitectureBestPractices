﻿using FluentValidation;

namespace App.Services.Products
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotNull().WithMessage("Ürün ismi gereklidir!")
                .NotEmpty().WithMessage("Ürün ismi boş olamaz!")
                .Length(3, 10).WithMessage("Ürün ismi 3 ile 10 karakter arasında olmalıdır!");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Ürün fiyatı 0'dan büyük olmalıdır!");

            RuleFor(x => x.Stock)
                .InclusiveBetween(1, 100).WithMessage("Ürün stok adeti 1 ile 100 arasında olmalıdır!");
        }
    }
}
