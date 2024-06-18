import { Component } from '@angular/core';
import { Category } from '../models/category.model';
import { Manufacturer } from '../models/manufacturer.model';
import { Product } from '../models/product.model';
import { CategoryService } from '../shared/category.service';
import { ManufacturerService } from '../shared/manufacturer.service';
import { ProductService } from '../shared/product.service';
import { Subscription } from 'rxjs';
import { Feedback } from '../models/feedback.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {
  popularCategories: Category[] = [];
  popularManufacturers: Manufacturer[] = [];
  popularProducts: Product[] = [];
  isLoading = false;

  categoriesSubscription: Subscription;
  manufacturersSubscription: Subscription;
  productsSubscription: Subscription;

  constructor(
    private categoryService: CategoryService,
    private manufacturerService: ManufacturerService,
    private productService: ProductService
  ) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.manufacturersSubscription = this.manufacturerService.getPopularManufacturers().subscribe({
      next: (manufacturers: Manufacturer[]) => {
        this.popularManufacturers = manufacturers;
        this.checkLoadingState();
      },
      error: (error) => {
        console.error('Error fetching manufacturers:', error);
        this.checkLoadingState();
      }
    });

    this.productsSubscription = this.productService.getPopularProducts().subscribe({
      next: (products: Product[]) => {
        this.popularProducts = products;
        this.checkLoadingState();
      },
      error: (error) => {
        console.error('Error fetching products:', error);
        this.checkLoadingState();
      }
    });

    this.categoriesSubscription = this.categoryService.getPopularCategories().subscribe({
      next: (categories: Category[]) => {
        this.popularCategories = categories;
        this.checkLoadingState();
      },
      error: (error) => {
        console.error('Error fetching categories:', error);
        this.checkLoadingState();
      }
    });
  }

  checkLoadingState() {
    if (this.popularManufacturers.length > 0 &&
      this.popularProducts.length > 0 && 
      this.popularCategories.length > 0
    ) {
      this.isLoading = false;
    }
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
    if (this.categoriesSubscription) {
      this.categoriesSubscription.unsubscribe();
    }
    if (this.manufacturersSubscription) {
      this.manufacturersSubscription.unsubscribe();
    }
    if (this.productsSubscription) {
      this.productsSubscription.unsubscribe();
    }
  }
}
