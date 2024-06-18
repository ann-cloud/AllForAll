import { Component, ViewChild } from '@angular/core';
import { Product } from '../models/product.model';
import { Subscription } from 'rxjs';
import { ProductService } from '../shared/product.service';
import { ActivatedRoute, Params } from '@angular/router';
import { UserService } from '../shared/user.service';
import { User } from '../models/user.model';
import { NgForm } from '@angular/forms';
import { FeedbackService } from '../shared/feedback.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrl: './product.component.css'
})
export class ProductComponent {
  product: Product;
  rating = 0;
  users: User[] = [];
  id: number = 0;
  productSubscription: Subscription;
  userSubscription: Subscription;
  feedbackSubscription: Subscription;
  isLoading = false;

  @ViewChild('f', { static: false }) slForm: NgForm;
  selectedRating = 1;
  feedbackText: string;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private userService: UserService,
    private feedbackService: FeedbackService
  ) { }


  ngOnInit(): void {
    this.route.params.subscribe((params: Params) => {
      this.id = +params['productId'];
      this.isLoading = true;

      this.fetchProductInfo();
    });
  }

  fetchProductInfo() {
    this.productSubscription = this.productService.getProductById(this.id).subscribe({
      next: (product: Product) => {
        this.product = product;
        let sum = 0;
        this.product.feedbacks.forEach(feedback => {
          sum += feedback.rating;
          this.userSubscription = this.userService.getUserById(feedback.userId).subscribe({
            next: (user: User) => {
              feedback.user = user;
              this.isLoading = false;
            },
            error: (error) => {
              console.error(`Error fetching user with id ${feedback.userId}`, error);
              this.isLoading = false;
            }
          });
        });
        this.product.feedbacks.sort((a, b) => {
          return new Date(b.feedbackDate).getTime() - new Date(a.feedbackDate).getTime();
        });
        this.rating = product.feedbacks.length == 0 ? 0 : sum / product.feedbacks.length;
        this.isLoading = false;
      },
      error: (error) => {
        console.error(`Error fetching product with id ${this.id}`, error);
        this.isLoading = false;
      }
    });
  }

  getStarColor(rating: number, starIndex: number): string {
    if (rating >= starIndex) {
      return '#F39C12';
    } else {
      return '#D9D9D9';
    }
  }  

  onSubmit(form: NgForm) {
    const feedbackDate = new Date();
    feedbackDate.setHours(feedbackDate.getHours() - feedbackDate.getTimezoneOffset() / 60);
    const feedbackData = {
      productId: this.id, 
      userId: 2, 
      rating: form.value.stars,
      comment: form.value.feedback,
      feedbackDate: feedbackDate
    };
    this.isLoading = true;
    this.feedbackSubscription = this.feedbackService.postFeedback(feedbackData).subscribe({
      next: (feedbackId: number) => {
        this.fetchProductInfo();
        this.isLoading = false;
      },
      error: (error) => {
        console.error(`Error publishing feedback`, error);
        this.isLoading = false;
      }
    });
    form.reset();
  }

  ngOnDestroy() {
    if (this.productSubscription) {
      this.productSubscription.unsubscribe();
    }
    if (this.userSubscription) {
      this.userSubscription.unsubscribe();
    }
  }
}
