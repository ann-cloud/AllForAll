import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductAddComponent } from './productsadd.component';

describe('ProductComponent', () => {
  let component: ProductAddComponent;
  let fixture: ComponentFixture<ProductAddComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ProductAddComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ProductAddComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
