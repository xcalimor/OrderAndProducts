import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component'
import { ProductListComponent} from './product-list/product-list.component'
import { ProductComponent } from './product/product.component';
import { AuthguardService } from './services/authguard.service';
const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'products', component: ProductListComponent, canActivate: [AuthguardService]},
  {path: 'products/:id', component: ProductComponent, canActivate: [AuthguardService]},
  {path: '', component: LoginComponent},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
