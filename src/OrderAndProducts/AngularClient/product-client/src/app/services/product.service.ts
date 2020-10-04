import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Product } from '../interfaces/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  baseUrl: string = "https://localhost:5104/api/product";
  constructor(private http: HttpClient) { }

  getProducts() {
    return this.http.get<Product[]>(this.baseUrl);
  }

  getProduct(id) {
    return this.http.get<Product>(`${this.baseUrl}/${id}`);
  }

  updateProduct(id, name, inStock, url){
    return this.http.put<any>(`${this.baseUrl}/${id}`, {name: name, instock: inStock, pictureurl: url});
  }

  createProduct(name, inStock, url){
    return this.http.put<any>(this.baseUrl, {name: name, instock: inStock, pictureurl: url});
  }

  deleteProduct(id) {
    return this.http.delete<any>(`${this.baseUrl}/${id}`);
  }

}
