import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { Category } from '../models/category.model';
import { Manufacturer } from '../models/manufacturer.model';
import { CategoryService } from '../shared/category.service';
import { ManufacturerService } from '../shared/manufacturer.service';
import { Subscription } from 'rxjs';
import { ProductService } from '../shared/product.service';
import { UserService } from '../shared/user.service'
import { User } from '../models/user.model';
@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent {
  isAuthenticated = false;
  isLogin = false;
  showModal = false;
  categories: Category[];
  manufacturers: Manufacturer[];
  countries: string[];
  categorySubscription: Subscription;
  manufacturerSubscription: Subscription;
  countrySubscription: Subscription;
  userData: User;

  constructor(
    private categoryService: CategoryService,
    private manufacturerService: ManufacturerService,
    private productService: ProductService,
    private userService: UserService,
    private router: Router,
  ) { }

  ngOnInit() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event: NavigationEnd) => {
      this.isLogin = event.url.includes('/login');
      console.log(this.isLogin);
      const token = localStorage.getItem('token');
      if (token) {
        this.userService.getCheckToken().subscribe({
          next: (user) => {
            this.userData = user;
            this.isAuthenticated = true;
          },
          error: (error) => {
            console.error('Error checking token:', error);
            this.userData = null;
            this.isAuthenticated = false;
            localStorage.removeItem('token');
          }
        });
      }
      else{
        this.userData = null;
        this.isAuthenticated = false;
      }
    });

    // Getting categories
    this.categorySubscription = this.categoryService.getCategories().subscribe({
      next: (categories: Category[]) => {
        this.categories = categories; 
      },
      error: (error) => {
        console.error('Error fetching categories:', error); 
      }
    });

    // Getting manufacturers
    this.manufacturerSubscription = this.manufacturerService.getManufacturers().subscribe({
      next: (manufacturers: Manufacturer[]) => {
        this.manufacturers = manufacturers; 
      },
      error: (error) => {
        console.error('Error fetching manufacturers:', error); 
      }
    });

    // Getting countries
    this.countrySubscription = this.productService.countries$.subscribe({
      next: (countries: string[]) => {
        this.countries = countries; 
      },
      error: (error) => {
        console.error('Error fetching countries:', error); 
      }
    });
    this.userService.getCheckToken().subscribe({
      next: (user) => {
        // Do something with the user object if needed
        console.log(user);
      },
      error: (error) => {
        console.error('Error checking token:', error);
      }
    });
    
  }

  ngOnDestroy() {
    this.categorySubscription.unsubscribe();
    this.manufacturerSubscription.unsubscribe();
    this.countrySubscription.unsubscribe();
    
  }
  logout() {
    this.userService.logout();
    this.userData = null;
    this.isAuthenticated = false;
  }

}
