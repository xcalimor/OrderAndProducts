import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from '../interfaces/product';
import { LoginService } from '../services/login.service';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: Product[]
  constructor(private productService: ProductService, 
    private loginService: LoginService,
    private router: Router) { }

  ngOnInit(): void {
    this.productService.getProducts().subscribe(result => {
      this.products = result;
    })
  }

  logout(){
    this.loginService.logoutUser();
    this.router.navigate(['/']);
  }

}
