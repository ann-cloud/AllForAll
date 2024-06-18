import { Component, ElementRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { CategoryService } from '../shared/category.service';
import { Subscription } from 'rxjs';
import { Category } from '../models/category.model';
import { ManufacturerService } from '../shared/manufacturer.service';
import { ProductService } from '../shared/product.service';
import { Manufacturer } from '../models/manufacturer.model';
import { Product } from '../models/product.model';
import { Feedback } from '../models/feedback.model';

@Component({
  selector: 'app-products-section',
  templateUrl: './products-section.component.html',
  styleUrls: ['./products-section.component.css']
})
export class ProductsSectionComponent {
  section = '';
  id: number = 0;
  isLoading: boolean = false;
  @ViewChild('verification') verificationRef: ElementRef<HTMLInputElement>;

  categorySubscription: Subscription;
  manufacturerSubscription: Subscription;
  productsSubscription: Subscription;

  category: Category;
  manufacturer: Manufacturer;
  country: string = "Ukraine 🤩";

  manufacturers: Manufacturer[] = [];
  categories: Category[] = [];
  countries: string[] = [];
  products: Product[] = [];
  savedProducts: Product[] = [];

  selectedCategories: string[] = [];
  selectedManufacturers: string[] = [];
  selectedCountries: string[] = [];

  constructor(
    private route: ActivatedRoute,
    private categoryService: CategoryService,
    private manufacturerService: ManufacturerService,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    this.route.url.subscribe(segments => {
      const segmentPaths = segments.map(segment => segment.path);
      this.section = segmentPaths[0];
    });

    this.route.params.subscribe((params: Params) => {
      if (this.section == "countries") {
        this.country = params['name'];
        this.isLoading = true;

        this.productsSubscription = this.productService.getFilteredProducts(this.section, this.country).subscribe({
          next: (products: Product[]) => {
            this.products = products;
            this.savedProducts = products;
            this.categories = products.map(product => product.category)
              .filter((category, index, self) =>
                index === self.findIndex(c => c.categoryId === category.categoryId) // Filter out duplicates
              );
            this.manufacturers = products.map(product => product.manufacturer)
              .filter((manufacturer, index, self) =>
                index === self.findIndex(m => m.manufacturerId === manufacturer.manufacturerId) // Filter out duplicates
              );
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error fetching products:', error);
            this.isLoading = false;
          }
        });
      }
      else if (this.section == "categories") {
        this.id = +params['id'];
        this.isLoading = true;

        this.categorySubscription = this.categoryService.getCategoryById(this.id).subscribe({
          next: (category: Category) => {
            this.category = category;

            this.productsSubscription = this.productService.getFilteredProducts(this.section, this.category.name).subscribe({
              next: (products: Product[]) => {
                this.products = products;
                this.savedProducts = products;
                this.manufacturers = products.map(product => product.manufacturer)
                  .filter((manufacturer, index, self) =>
                    index === self.findIndex(m => m.manufacturerId === manufacturer.manufacturerId) // Filter out duplicates
                  );
                this.countries = products.map(product => product.country)
                  .filter((country, index, self) =>
                    index === self.findIndex(c => c === country) // Filter out duplicates
                  );
                this.isLoading = false;
              },
              error: (error) => {
                console.error('Error fetching products:', error);
                this.isLoading = false;
              }
            });
          },
          error: (error) => {
            console.error(`Error fetching category with id ${this.id}`, error);
            this.isLoading = false;
          }
        });
      } else if (this.section == "manufacturers") {
        this.id = +params['id'];
        this.isLoading = true;

        this.manufacturerSubscription = this.manufacturerService.getManufacturerById(this.id).subscribe({
          next: (manufacturer: Manufacturer) => {
            this.manufacturer = manufacturer;

            this.productsSubscription = this.productService.getFilteredProducts(this.section, this.manufacturer.name).subscribe({
              next: (products: Product[]) => {
                this.products = products;
                this.savedProducts = products;
                this.categories = products.map(product => product.category)
                  .filter((category, index, self) =>
                    index === self.findIndex(c => c.categoryId === category.categoryId) // Filter out duplicates
                  );
                this.countries = products.map(product => product.country)
                  .filter((country, index, self) =>
                    index === self.findIndex(c => c === country) // Filter out duplicates
                  );
                this.isLoading = false;
              },
              error: (error) => {
                console.error('Error fetching products:', error);
                this.isLoading = false;
              }
            });
          },
          error: (error) => {
            console.error(`Error fetching manufacturer with id ${this.id}`, error);
            this.isLoading = false;
          }
        });
      }
    });

  }

  // Filter functions
  onCountriesChange(country: string, isChecked: boolean): void {
    if (isChecked) {
      this.selectedCountries.push(country);
    } else {
      this.selectedCountries = this.selectedCategories.filter(c => c !== country);
    }

    this.filterProducts();
  }

  onCategoriesChange(categoryName: string, isChecked: boolean): void {
    if (isChecked) {
      this.selectedCategories.push(categoryName);
    } else {
      this.selectedCategories = this.selectedCategories.filter(c => c !== categoryName);
    }

    this.filterProducts();
  }

  onManufacturersChange(manufacturerName: string, isChecked: boolean): void {
    if (isChecked) {
      this.selectedManufacturers.push(manufacturerName);
    } else {
      this.selectedManufacturers = this.selectedManufacturers.filter(m => m !== manufacturerName);
    }

    this.filterProducts();
  }

  onVerificationChange(): void {
    this.filterProducts();
  }

  filterProducts(): void {
    let filteredProducts = this.savedProducts; // Start with all products

    // Filter by Categories
    if (this.selectedCategories.length > 0) {
      filteredProducts = filteredProducts.filter(product => this.selectedCategories.includes(product.category.name));
    }

    // Filter by Manufacturers (similar logic as categories)
    if (this.selectedManufacturers.length > 0) {
      filteredProducts = filteredProducts.filter(product => this.selectedManufacturers.includes(product.manufacturer.name));
    }

    // Filter by Countries (similar logic as categories)
    if (this.selectedCountries.length > 0) {
      filteredProducts = filteredProducts.filter(product => this.selectedCountries.includes(product.country));
    }

    // Filter by Verification (if applicable)
    if (this.verificationRef.nativeElement.checked) {
      filteredProducts = filteredProducts.filter(product => product.isVerified);
    }
    
    this.products = filteredProducts;
  }

  countRating(feedbacks: Feedback[]) {
    let sum = 0;
    feedbacks.forEach(feedback => {
      sum += feedback.rating;
    });
    return feedbacks.length == 0 ? 0 : sum / feedbacks.length;
  }

  getStarColor(rating: number, starIndex: number): string {
    if (rating >= starIndex) {
      return '#F39C12';
    } else {
      return '#D9D9D9';
    }
  }  

  ngOnDestroy(): void {
    if (this.categorySubscription) {
      this.categorySubscription.unsubscribe();
    }
    if (this.manufacturerSubscription) {
      this.manufacturerSubscription.unsubscribe();
    }
    if (this.productsSubscription) {
      this.productsSubscription.unsubscribe();
    }
  }

  currentPage = 1;
  itemsPerPage = 9;

  get startIndex() {
    return (this.currentPage - 1) * this.itemsPerPage;
  }

  get endIndex() {
    return Math.min(this.startIndex + this.itemsPerPage - 1, this.products.length - 1);
  }

  get paginatedItems() {
    return this.products.slice(this.startIndex, this.endIndex + 1);
  }

  onPageChange(pageNumber: number) {
    this.currentPage = pageNumber;
  }
}
