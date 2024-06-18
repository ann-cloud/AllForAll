import { Component } from '@angular/core';
import { ProductService } from '../shared/product.service';
import { ProductRequestDto } from '../models/productdto.model';
@Component({
  selector: 'app-productadd',
  templateUrl: './productsadd.component.html',
  styleUrls: ['./productsadd.component.css']
})
export class ProductAddComponent {
  productName: string;
  country: string;
  fileToUpload: File = null;
  selectedManufacturer: number;
  selectedCategory: number;
  manufacturers = [
    { id: 3, name: 'Samsung' },
    { id: 4, name: 'Levis' },
    { id: 5, name: 'Penguin Random House' },
    { id: 6, name: 'IKEA' },
    { id: 7, name: 'Nike' },
    { id: 8, name: 'Hasbro' },
    { id: 9, name: 'LOréal' },
    { id: 10, name: 'Nestlé' },
    { id: 11, name: 'Mars, Incorporated' },
    { id: 12, name: 'Sony' },
    { id: 13, name: 'Adidas' },
    { id: 14, name: 'LEGO' },
    { id: 15, name: 'Maybelline' },
    { id: 16, name: 'Unilever' },
    { id: 17, name: 'Canon' },
    { id: 18, name: 'Ferrero' },
    { id: 19, name: 'Microsoft' }
  ];

  categories = [
    { id: 1, name: 'Electronics' },
    { id: 2, name: 'Clothing' },
    { id: 3, name: 'Books' },
    { id: 4, name: 'Home Decor' },
    { id: 5, name: 'Sports' },
    { id: 6, name: 'Toys' },
    { id: 7, name: 'Furniture' },
    { id: 8, name: 'Beauty' },
    { id: 9, name: 'Food' }
  ];

  constructor(private productService: ProductService) { }

  onSubmit(form): void {
    const product: ProductRequestDto = {
      productName: this.productName,
      country: this.country,
      manufacturer: this.selectedManufacturer,
      category: this.selectedCategory,
    };
    console.log(product);
    if(product.productName.length <= 5 || product.country.length <= 5 || !product.category ||  !product.manufacturer){
      return;
    }
    if (form.valid) {

      this.productService.createProduct(product, this.fileToUpload).subscribe(
        (response) => {
          console.log('Product created successfully:', response);
          alert('Product created successfully!');
        },
        (error) => {
          console.error('Error creating product:', error);
        }
      );
    }
  }

  onFileChange(event): void {
    this.fileToUpload = event.target.files[0];
  }
}
