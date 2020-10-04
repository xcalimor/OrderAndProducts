import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Product } from '../interfaces/product';
import { LoginService } from '../services/login.service';
import { ProductService } from '../services/product.service';

@Component({
  selector: 'app-product',
  templateUrl: './product.component.html',
  styleUrls: ['./product.component.css']
})
export class ProductComponent implements OnInit {
  product: Product;
  isAdmin: boolean;
  constructor(private route: ActivatedRoute, 
    private productService: ProductService, 
    private loginService: LoginService,
    private router: Router) { }

  ngOnInit(): void {
    console.log("role=" + this.loginService.getUserRole());
    this.isAdmin = this.loginService.getUserRole() === "Admin"
    this.route.params
    .subscribe(params => {
      const prodId = params['id']
      this.productService.getProduct(prodId).subscribe(result=> {
        this.product = result;
      })
    });
  }

  deleteProduct(){
    this.productService.deleteProduct(this.product.id).subscribe(() => {
      this.router.navigate(['/products'])
    });
  }
  
  logout(){
    this.loginService.logoutUser();
    this.router.navigate(['/']);
  }

}
